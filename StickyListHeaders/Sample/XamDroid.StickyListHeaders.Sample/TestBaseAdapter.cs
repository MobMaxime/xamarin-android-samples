using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using Java.Lang;
using com.refractored.components.stickylistheaders;

namespace StickyListHeaders
{
    public class HeaderViewHolder : Java.Lang.Object 
    {
        public TextView Text1 { get; set; }
    }

    public class ViewHolder : Java.Lang.Object
    {
        public TextView Text { get; set; }
    }


	public class TestBaseAdapter : BaseAdapter<TransactionItems>, IStickyListHeadersAdapter//, ISectionIndexer
    {
//        private int[] m_SectionIndices;
//		private string[] m_SectionLetters;
        private LayoutInflater m_Inflater;
        private Context m_Context;
//        private Java.Lang.Object[] m_SectionLettersLang;
		List<TransactionItems> _lstCustomTransactionItem ;


        public TestBaseAdapter(Context context)
        {
            m_Context = context;
            m_Inflater = LayoutInflater.From(context);
			SetTransaction ();
        }

        public override int Count
        {
			get { return _lstCustomTransactionItem.Count; }
        }

		public override TransactionItems this[int index] {
			get {
				return _lstCustomTransactionItem[index];
			}
		}

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ViewHolder holder = null;
            if (convertView == null)
            {
                holder = new ViewHolder();
                convertView = m_Inflater.Inflate(Resource.Layout.test_list_item_layout, parent, false);
                holder.Text = convertView.FindViewById<TextView>(Resource.Id.text);
                convertView.Tag = holder;
            }
            else
            {
                holder = convertView.Tag as ViewHolder;
            }
			holder.Text.Text = _lstCustomTransactionItem[position].Country;

            return convertView;

        }


        public View GetHeaderView(int position, View convertView, ViewGroup parent)
        {
            HeaderViewHolder holder = null;
            if (convertView == null)
            {
                holder = new HeaderViewHolder();
                convertView = m_Inflater.Inflate(Resource.Layout.header, parent, false);
                holder.Text1 = convertView.FindViewById<TextView>(Resource.Id.text1);
                convertView.Tag = holder;
            }
            else
            {
                holder = convertView.Tag as HeaderViewHolder;
            }

			holder.Text1.Text = _lstCustomTransactionItem[position].Headerkey;
            return convertView;
        }

		public string GetHeaderId(int position)
        {
			return _lstCustomTransactionItem [position].Headerkey;
        }

		void SetTransaction()
		{
			List<TransactionItems> lstTransaction = new List<TransactionItems> ();
			lstTransaction.Add (new TransactionItems () {EventName = "Africa Food Crisis",
				Country = "Kenya",
				Level = "AED",
				Qty = "500"
			});

			lstTransaction.Add (new TransactionItems () {EventName = "Food Crisis",
				Country = "India",
				Level = "AED",
				Qty = "5000"
			});

			lstTransaction.Add (new TransactionItems () {EventName = " Crisis",
				Country = "Us",
				Level = "AED",
				Qty = "1050"
			});

			Dictionary<string, List<TransactionItems>> TransactionItemsDic = new Dictionary<string, List<TransactionItems>> ();
			TransactionItemsDic.Add ("1 MAY 2014", lstTransaction);
			TransactionItemsDic.Add ("5 JUNE 2014", lstTransaction);
			TransactionItemsDic.Add ("11 AUG 2015", lstTransaction);
			TransactionItemsDic.Add ("132 MAY 2014", lstTransaction);
			TransactionItemsDic.Add ("5234 JUNE 2014", lstTransaction);
			TransactionItemsDic.Add ("11234 AUG 2015", lstTransaction);
			List<TransactionItems> lstCustomTransactionItem = new List<TransactionItems>();
			TransactionItems obj = new TransactionItems (); 

			foreach (var item in TransactionItemsDic) {
				for (int i = 0; i < item.Value.Count; i++) {
					obj = new TransactionItems(); 
					TransactionItems obj1 = new TransactionItems(); 
					obj =  item.Value [i];
					obj.Headerkey = item.Key;
					obj1 = (TransactionItems)obj.Clone();
					lstCustomTransactionItem.Add(obj1);
					obj = null;
				}
			}
			_lstCustomTransactionItem = lstCustomTransactionItem;
		}
    }
	public class TransactionItems :Java.Lang.Object,ICloneable
	{
		public TransactionItems ()
		{

		}
		public object Clone()
		{
			return this.MemberwiseClone();
		}

		public string Headerkey{ get; set;}

		public string EventName { get; set; }

		public string Country { get; set; }

		public string Level { get; set; }

		public string Qty { get; set; }

		public int TrasactionImage{ get; set;}
	}
}