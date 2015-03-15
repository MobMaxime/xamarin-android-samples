
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

using Android.Content;
using Android.Widget;
using Java.Interop;

namespace com.refractored.components.stickylistheaders
{
    /// <summary>
    /// Wrapper for section indexer
    /// </summary>
    public class SectionIndexerAdapterWrapper : AdapterWrapper, ISectionIndexer
    {
        private ISectionIndexer m_SectionIndexer;

        public SectionIndexerAdapterWrapper(Context context, ISectionIndexer indexer) : base(context, indexer)
        {
            m_SectionIndexer = indexer;
        }


        public int GetPositionForSection(int section)
        {
            return m_SectionIndexer.GetPositionForSection(section);
        }

        public int GetSectionForPosition(int position)
        {
            return m_SectionIndexer.GetSectionForPosition(position);
        }

        public Java.Lang.Object[] GetSections()
        {
            return m_SectionIndexer.GetSections();
        }
    }
}