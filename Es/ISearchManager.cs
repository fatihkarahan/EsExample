using System;
using System.Collections.Generic;
using System.Text;
using Es.Models;

namespace Es
{
    public interface ISearchManager
    {
        SearchResultModel SearchProducts(SearchParameters parameters);
        void AddOrUpdate<T>(T model) where T : ProductModel;
        void CreateIndex();
    }
}
