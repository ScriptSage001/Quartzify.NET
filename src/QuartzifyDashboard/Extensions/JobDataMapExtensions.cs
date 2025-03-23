#region License

/*
 * All content copyright Kaustab Samanta, unless otherwise indicated. All rights reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy
 * of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 *
 */

#endregion

using Quartz;

namespace QuartzifyDashboard.Extensions;

/// <summary>
/// Provides extension methods for converting Quartz JobDataMap to a Dictionary.
/// </summary>
public static class JobDataMapExtensions
{
    /// <summary>
    /// Converts a Quartz JobDataMap to a Dictionary with string keys and object values.
    /// </summary>
    /// <param name="map">The JobDataMap to convert.</param>
    /// <returns>A dictionary containing all key-value pairs from the JobDataMap.</returns>
    public static Dictionary<string, object> ToDictionary(this JobDataMap map)
    {
        var result = new Dictionary<string, object>();
        foreach (var key in map.Keys)
        {
            result[key] = map[key];
        }
        return result;
    }
}