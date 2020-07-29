// <copyright file="PropertyHandler.cs" company="Firoozeh Technology LTD">
// Copyright (C) 2020 Firoozeh Technology LTD. All Rights Reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//    limitations under the License.
// </copyright>


/**
* @author Alireza Ghodrati
*/


using System;
using System.Collections.Generic;
using System.Linq;
using FiroozehGameService.Models;
using FiroozehGameService.Models.GSLive;
using Plugins.GameService.Utils.RealTimeUtil.Interfaces;

namespace Plugins.GameService.Utils.RealTimeUtil.Classes.Handlers
{
    internal class PropertyHandler : IPropertyHandler
    {
        
        private Dictionary<string, Dictionary<string, object>> _propertiesCache;
        private IMemberHandler _memberHandler;
        
        public void Init(IMemberHandler memberHandler)
        {
            _memberHandler = memberHandler;
            _propertiesCache = new Dictionary<string, Dictionary<string, object>>();
        }

        public void Dispose()
        {
            _propertiesCache?.Clear();
        }

        public void ApplyProperty(string memberId, Tuple<string, object> property)
        {
            var (pName, pData) = property;
            if (!_propertiesCache.ContainsKey(memberId))
            {
                _propertiesCache.Add(memberId, new Dictionary<string, object>());
                _propertiesCache[memberId].Add(pName,pData);
            }
            else
            {
                if (_propertiesCache[memberId].ContainsKey(pName))
                    _propertiesCache[memberId][pName] = pData;
                else
                    _propertiesCache[memberId].Add(pName,pData);
            }
        }

        public void RemoveProperty(string memberId, string propertyName)
        {
            if(_propertiesCache.ContainsKey(memberId))
                _propertiesCache[memberId]?.Remove(propertyName);
        }

        public Dictionary<string, object> GetMemberProperties(string memberId)
        {
            if(!_propertiesCache.ContainsKey(memberId))
                throw new GameServiceException("memberId " + memberId + " is Not Exist!");

            return _propertiesCache[memberId];
        }

        public List<Member> GetPropertyMembers(string propertyName, object propertyData)
        {
            var ids = _propertiesCache
                .ToList()
                .FindAll(pc
                    => pc.Value.ContainsKey(propertyName) && pc.Value[propertyName] == propertyData)
                .Select(pc => pc.Key)
                .ToList();

            return _memberHandler
                .GetAllMembers()
                .FindAll(m => ids.Contains(m.Id));
        }

        public List<Member> GetPropertyMembers(string propertyName)
        {
            var ids = _propertiesCache
                .ToList()
                .FindAll(pc => pc.Value.ContainsKey(propertyName))
                .Select(pc => pc.Key)
                .ToList();
            
            return _memberHandler
                .GetAllMembers()
                .FindAll(m => ids.Contains(m.Id));
        }

        public List<object> GetPropertyValues(string propertyName)
        {
            return _propertiesCache
                .ToList()
                .FindAll(pc => pc.Value.ContainsKey(propertyName))
                .Select(pc => pc.Value[propertyName])
                .ToList();
        }
    }
}