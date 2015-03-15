/*
 * Copyright (C) 2013 @JamesMontemagno http://www.montemagno.com http://www.refractored.com
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 * Converted from: https://github.com/emilsjolander/StickyListHeaders
 */

using Android.Views;
using Android.Widget;

namespace com.refractored.components.stickylistheaders
{
    public interface IStickyListHeadersAdapter : IListAdapter
    {
        /// <summary>
        /// Get a View that displays the header data at the specified position in the
        /// set. You can either create a view manually or inflate it from an XML layout file
        /// </summary>
        /// <param name="position">The position of the item within the adapter's data set of the item whose
        /// header view we want.</param>
        /// <param name="convertView">The old view to reuse, if possible. Note: you shoudl check that this is
        /// non-null and of an approriate type before using. If it is not possible to convert the view
        /// to display the correct data this method can create a new view</param>
        /// <param name="parent">The parent that this view will evenaually attach to</param>
        /// <returns>A view corresponding to the data at the specified position</returns>
        View GetHeaderView(int position, View convertView, ViewGroup parent);
        
        /// <summary>
        /// Get the header id associated with the specificed position in the list
        /// </summary>
        /// <param name="position">The position of the item within the adapter's data set whose header id we want</param>
        /// <returns>The id of the header at the specified position</returns>
		string GetHeaderId(int position);
    }
}