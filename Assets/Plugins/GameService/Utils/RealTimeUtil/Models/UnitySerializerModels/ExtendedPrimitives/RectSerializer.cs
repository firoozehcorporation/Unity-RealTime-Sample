// <copyright file="RectSerializer.cs" company="Firoozeh Technology LTD">
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


using FiroozehGameService.Utils.Serializer.Abstracts;
using FiroozehGameService.Utils.Serializer.Helpers;
using UnityEngine;

namespace Plugins.GameService.Utils.RealTimeUtil.Models.UnitySerializerModels.ExtendedPrimitives
{
    internal class RectSerializer : ObjectSerializer<Rect>
    {
        protected override void WriteObject(Rect obj, GsWriteStream writeStream)
        {
            // NOTE : Must Register Type Vector2 Before this
            writeStream.WriteNext(obj.position);
            writeStream.WriteNext(obj.size);
        }

        protected override Rect ReadObject(GsReadStream readStream)
        {
            // NOTE : Must Register Type Vector2 Before this
            var position = (Vector2) readStream.ReadNext();
            var size     = (Vector2) readStream.ReadNext();

            return new Rect(position,size);
        }
    }
}