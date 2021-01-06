using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Nest;
using Es.Models;
using Es;

namespace Nop.ES
{
    public class SearchManager : ESManager, ISearchManager
    {
        public virtual void CreateIndex()
        {
            if (Client.Indices.Exists("product").Exists)
                return;

            var result = Client.Indices.Create("product",
                    ss => ss.Index("product")
                    .Settings(o => o.NumberOfShards(4).NumberOfReplicas(2).Setting("max_result_window", int.MaxValue))
                    .Mappings(m => m.Map<ProductModel>(
                        mm => mm.AutoMap().Properties(
                            p => p.Text(
                                t => t.Name(n => n.Name)
                                ))
                        )));
            if (result.Acknowledged)
            {
                Client.Indices.PutAlias("product", "product");
                return;
            }
            return;
        }
        public void DeleteIndex()
        {

            var indexResponse = Client.Indices.Delete("product");
        }

        /// <summary>
        /// search elastic
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public SearchResultModel SearchProducts(SearchParameters parameters)
        {
            var mustClauses = new List<QueryContainer>();
            #region query
            if (parameters.Q != null && !string.IsNullOrEmpty(parameters.Q))
                mustClauses.Add(new WildcardQuery
                {
                    Field = "name",
                    Value = "*" + parameters.Q + "*"
                });

            if (parameters.ManufacturerIds != null)
                mustClauses.Add(new TermsQuery
                {
                    Field = "manufacturerid",
                    Terms = parameters.ManufacturerIds.Split(",")
                });

            if (parameters.VendorId > 0)
                mustClauses.Add(new TermQuery
                {
                    Field = new Field("vendor.id"),
                    Value = parameters.VendorId
                });


            if (parameters.SelectedCatId > 0)
                mustClauses.Add(new NestedQuery
                {
                    Path = "categories.category",
                    Query = new QueryContainer(new TermQuery
                    {
                        Field = new Field("categories.category.id"),
                        Value = parameters.SelectedCatId
                    })
                });

            if (parameters.CategoryIds != null && parameters.CategoryIds != "0")
                mustClauses.Add(new NestedQuery
                {
                    Path = "categories.category",
                    Query = new QueryContainer(new TermsQuery
                    {
                        Field = new Field("categories.category.id"),
                        Terms = parameters.CategoryIds.Split(",")
                    })
                });

            if (parameters.ColorSpecIds != null && parameters.ColorSpecIds.Count() > 0)
                mustClauses.Add(new NestedQuery
                {
                    Path = "colorspecs.spec",
                    Query = new QueryContainer(new TermsQuery
                    {
                        Field = new Field("colorspecs.spec.id"),
                        Terms = parameters.ColorSpecIds.ToArray().Select(c => c.ToString())
                    })
                });

            if (parameters.MaterialSpecIds != null && parameters.MaterialSpecIds.Count() > 0)
                mustClauses.Add(new NestedQuery
                {
                    Path = "materialspecs.spec",
                    Query = new QueryContainer(new TermsQuery
                    {
                        Field = new Field("materialspecs.spec.id"),
                        Terms = parameters.MaterialSpecIds.ToArray().Select(c => c.ToString())
                    })
                });


            if (!parameters.IsShowCloseProduct)
                mustClauses.Add(new TermQuery
                {
                    Field = new Field("isopensale"),
                    Value = true
                });

            if (parameters.MaxPrice > 0 || parameters.MinPrice > 0)
                mustClauses.Add(new NumericRangeQuery
                {
                    Field = new Field("price"),
                    LessThanOrEqualTo = parameters.MaxPrice == -1 ? int.MaxValue : parameters.MaxPrice,
                    GreaterThanOrEqualTo = parameters.MinPrice == -1 ? 0 : parameters.MinPrice
                });

            #endregion
            #region agg
            var aggregations = new Dictionary<string, IAggregationContainer>();
            aggregations.Add("categories", new AggregationContainer { Nested = new NestedAggregation("category") { Path = "categories.category" }, Aggregations = new TermsAggregation("catids") { Field = "categories.category.id" } });
            aggregations.Add("vendor", new AggregationContainer { Terms = new TermsAggregation("vendor.id") { Field = "vendor.id" } });
            aggregations.Add("colorspecs", new AggregationContainer { Nested = new NestedAggregation("colorspecs") { Path = "colorspecs.spec" }, Aggregations = new TermsAggregation("specids") { Field = "colorspecs.spec.id" } });
            aggregations.Add("materialspecs", new AggregationContainer { Nested = new NestedAggregation("materialspecs") { Path = "materialspecs.spec" }, Aggregations = new TermsAggregation("specids") { Field = "materialspecs.spec.id" } });
            aggregations.Add("manufacturers", new AggregationContainer { Terms = new TermsAggregation("manufacturerid") { Field = "manufacturerid" } });
            aggregations.Add("maxprice", new AggregationContainer { Max = new MaxAggregation("price", new Field("price")) });
            aggregations.Add("minprice", new AggregationContainer { Min = new MinAggregation("price", new Field("price")) });
            #endregion

            #region sort
            var sort = new List<ISort>();
            if (parameters.Sortfield == "createdDate")
            {
                sort.Add(new FieldSort { Field = "createdDate", Order = SortOrder.Descending });
            }
            else
            {
                sort.Add(new FieldSort { Field = "price", Order = SortOrder.Ascending });
            }
            #endregion
            var searchRequest = new SearchRequest<ProductModel>("product")
            {
                Size = parameters.Size,
                From = (parameters.Page == 0 ? 0 : parameters.Page - 1) * parameters.Size,
                Query = new BoolQuery { Must = mustClauses },
                Aggregations = aggregations,
                Sort = sort,
                TrackTotalHits = true
            };
            var response = Client.Search<ProductModel>(searchRequest);
#if DEBUG
            var stream = new MemoryStream();
            Client.RequestResponseSerializer.Serialize(searchRequest, stream);
            var jsonQuery = Encoding.UTF8.GetString(stream.ToArray());
#endif
            return BuildSearchResponse(response);
        }

        /// <summary>
        /// es add or update products
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="indexName"></param>
        /// <param name="model"></param>
        public virtual void AddOrUpdate<T>(T model) where T : ProductModel
        {
            //CreateIndex();
            var exis = Client.DocumentExists(DocumentPath<T>.Id(new Id(model)), dd => dd.Index("product"));

            if (exis.Exists)
            {
                var result = Client.Update(DocumentPath<T>.Id(new Id(model)),
                    ss => ss.Index("product").Doc(model).RetryOnConflict(3));
            }
            else
            {
                var result = Client.Index(model, ss => ss.Index("product"));
            }
        }

        /// <summary>
        /// build response
        /// </summary>
        /// <param name="searchResponse"></param>
        /// <returns></returns>
        private SearchResultModel BuildSearchResponse(ISearchResponse<ProductModel> searchResponse)
        {
            SearchResultModel result = new SearchResultModel();
            result.Products = result.Products;
            result.HitCount = searchResponse.HitsMetadata == null ? 0 : searchResponse.HitsMetadata.Total.Value;
            if (searchResponse.HitsMetadata != null)
            {
                var categories = searchResponse.Aggregations.Nested("categories") != null ? searchResponse.Aggregations.Nested("categories").Terms("catids").Buckets : null;
                List<string> categoryList = new List<string>();
                foreach (var item in categories)
                {
                    result.Categories.Add(Convert.ToInt32(item.Key));
                }

                var colorSpecids = searchResponse.Aggregations.Nested("colorspecs") != null ? searchResponse.Aggregations.Nested("colorspecs").Terms("specids").Buckets : null;
                foreach (var item in colorSpecids)
                {
                    result.ColorSpecs.Add(Convert.ToInt32(item.Key));
                }

                var materialSpecids = searchResponse.Aggregations.Nested("materialspecs") != null ? searchResponse.Aggregations.Nested("materialspecs").Terms("specids").Buckets : null;
                foreach (var item in materialSpecids)
                {
                    result.MaterialSpecs.Add(Convert.ToInt32(item.Key));
                }

                var manufacturers = searchResponse.Aggregations.Terms("manufacturers");
                List<string> manufacturerList = new List<string>();
                foreach (var item in manufacturers.Buckets)
                {
                    result.Manufacturers.Add(Convert.ToInt32(item.Key));
                }
                if (searchResponse.Aggregations.Any(k => k.Key == "maxprice"))
                    result.MaxPrice = Convert.ToInt32(searchResponse.Aggregations.Sum("maxprice").Value);
                if (searchResponse.Aggregations.Any(k => k.Key == "minprice"))
                    result.MinPrice = Convert.ToInt32(searchResponse.Aggregations.Sum("minprice").Value);
            }

            return result;
        }
    }
}
