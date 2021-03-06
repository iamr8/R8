﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

using Newtonsoft.Json;
using R8.Lib.Validatable;

namespace R8.Lib.Search
{
    public abstract class SearchBase : ValidatableObject, ISearchBase
    {
        private string _pageNo;

        public virtual object this[string key]
        {
            get
            {
                var property = GetType().GetProperty(key);
                if (property == null)
                    throw new NullReferenceException($"{GetType().Name} haven't {nameof(key)} property.");

                return property.GetValue(this);
            }
            set
            {
                var property = GetType().GetProperty(key);
                if (property == null)
                    throw new NullReferenceException($"{GetType().Name} haven't {nameof(key)} property.");

                property.SetValue(this, value);
            }
        }

        public virtual int PageNo
        {
            get => string.IsNullOrEmpty(_pageNo) ? 1 : int.TryParse(_pageNo, out var page) ? page : 1;
            set => _pageNo = (value <= 0 ? 1 : value).ToString();
        }

        [JsonIgnore]
        public virtual int PageSize { get; set; } = 10;
    }
}