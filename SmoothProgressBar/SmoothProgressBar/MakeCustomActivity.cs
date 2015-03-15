using Android.App;
using Android.Widget;
using Android.Views.Animations;
using Android.OS;
using Android.Content.Res;
using Android.Util;

namespace SmoothProgressBar
{
	[Activity (Label = "MakeCustomActivity")]			
	public class MakeCustomActivity : Activity
	{
		Castorflex.SmoothProgressBar.SmoothProgressBar _progressBar;
		CheckBox _checkBoxMirror;
		CheckBox _checkBoxReversed;
		Spinner _spinnerInterpolators;
		SeekBar _seekBarSectionsCount;
		SeekBar _seekBarStrokeWidth;
		SeekBar _seekBarSeparatorLength;
		SeekBar _seekBarSpeed;
		SeekBar _seekBarFactor;
		TextView _textViewFactor;
		TextView _textViewSpeed;
		TextView _textViewStrokeWidth;
		TextView _textViewSeparatorLength;
		TextView _textViewSectionsCount;

		int _strokeWidth = 4;
		int _separatorLength=4;
		int _sectionsCount;
		float _factor = 1f;
		float _speed  = 1f;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			RequestWindowFeature (Android.Views.WindowFeatures.NoTitle);
			SetContentView(Resource.Layout.activity_custom);

			_progressBar = (Castorflex.SmoothProgressBar.SmoothProgressBar) FindViewById(Resource.Id.progressbar);
			_checkBoxMirror = FindViewById<CheckBox>(Resource.Id.checkbox_mirror);
			_checkBoxReversed = FindViewById<CheckBox>(Resource.Id.checkbox_reversed);
			_spinnerInterpolators = FindViewById<Spinner>(Resource.Id.spinner_interpolator);
			_seekBarSectionsCount = FindViewById<SeekBar>(Resource.Id.seekbar_sections_count);
			_seekBarStrokeWidth = FindViewById<SeekBar>(Resource.Id.seekbar_stroke_width);
			_seekBarSeparatorLength = FindViewById<SeekBar>(Resource.Id.seekbar_separator_length);
			_seekBarSpeed = FindViewById<SeekBar>(Resource.Id.seekbar_speed);
			_seekBarFactor = FindViewById<SeekBar>(Resource.Id.seekbar_factor);
			_textViewSpeed =FindViewById<TextView>(Resource.Id.textview_speed);
			_textViewSectionsCount = FindViewById<TextView>(Resource.Id.textview_sections_count);
			_textViewSeparatorLength =FindViewById<TextView>(Resource.Id.textview_separator_length);
			_textViewStrokeWidth = FindViewById<TextView>(Resource.Id.textview_stroke_width);
			_textViewFactor = FindViewById<TextView>(Resource.Id.textview_factor);

			FindViewById (Resource.Id.button_start).Click += (object sender, System.EventArgs e) => {
				_progressBar.ProgressiveStart();
			};
			FindViewById (Resource.Id.button_stop).Click += (object sender, System.EventArgs e) => {
				_progressBar.ProgressiveStop();
			};
			_seekBarFactor.ProgressChanged += (object sender, SeekBar.ProgressChangedEventArgs e) => {
				_factor = (e.Progress + 1) / 10f;
				_textViewFactor.Text="Factor: " + _factor;
				SetInterpolator(_spinnerInterpolators.SelectedItemPosition);
			};
			_seekBarSpeed.ProgressChanged += (object sender, SeekBar.ProgressChangedEventArgs e) => {
				_speed = ((float) e.Progress + 1) / 10;
				_textViewSpeed.Text="Speed: " + _speed;
				_progressBar.SetSmoothProgressDrawableSpeed(_speed);
				_progressBar.SetSmoothProgressDrawableProgressiveStartSpeed(_speed);
				_progressBar.SetSmoothProgressDrawableProgressiveStopSpeed(_speed);
			};
			_seekBarSectionsCount.ProgressChanged += (object sender, SeekBar.ProgressChangedEventArgs e) => {
				_sectionsCount = e.Progress + 1;
				_textViewSectionsCount.Text="Sections count: " + _sectionsCount;
				_progressBar.SetSmoothProgressDrawableSectionsCount(_sectionsCount);
			};
			_seekBarSeparatorLength.ProgressChanged += (object sender, SeekBar.ProgressChangedEventArgs e) => {
				_separatorLength = e.Progress;
				_textViewSeparatorLength.Text=string.Format("Separator length: {0}dp", _separatorLength);
				_progressBar.SetSmoothProgressDrawableSeparatorLength(DpToPx(_separatorLength));
			};
			_seekBarStrokeWidth.ProgressChanged += (object sender, SeekBar.ProgressChangedEventArgs e) => {
				_strokeWidth =e.Progress;
				_textViewStrokeWidth.Text=string.Format("Stroke width: {0}dp", _strokeWidth);
				_progressBar.SetSmoothProgressDrawableStrokeWidth(DpToPx(_strokeWidth));
			};
			_checkBoxMirror.CheckedChange += (object sender, CompoundButton.CheckedChangeEventArgs e) => {
				_progressBar.SetSmoothProgressDrawableMirrorMode(e.IsChecked);
			};
			_checkBoxReversed.CheckedChange += (object sender, CompoundButton.CheckedChangeEventArgs e) => {
				_progressBar.SetSmoothProgressDrawableReversed(e.IsChecked);
			};
			_seekBarSeparatorLength.Progress=4;
			_seekBarSectionsCount.Progress=4;
			_seekBarStrokeWidth.Progress=4;
			_seekBarSpeed.Progress=9;
			_seekBarFactor.Progress=9;

			_spinnerInterpolators.Adapter=new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, Resources.GetStringArray(Resource.Array.interpolators));
			_spinnerInterpolators.ItemSelected += (object sender, AdapterView.ItemSelectedEventArgs e) => {
				SetInterpolator(e.Position);
			};    _spinnerInterpolators.SetSelection(0);
		}
		void SetInterpolator(int position) {
			IInterpolator CurrentInterpolator;
			switch (position) {
			case 1:
				CurrentInterpolator = new LinearInterpolator();
				_seekBarFactor.Enabled=false;
				break;
			case 2:
				CurrentInterpolator = new AccelerateDecelerateInterpolator();
				_seekBarFactor.Enabled=false;
				break;
			case 3:
				CurrentInterpolator = new DecelerateInterpolator(_factor);
				_seekBarFactor.Enabled=true;
				break;
			case 0:
			default:
				CurrentInterpolator = new AccelerateInterpolator(_factor);
				_seekBarFactor.Enabled=true;
				break;
			}

			_progressBar.SetSmoothProgressDrawableInterpolator(CurrentInterpolator);
			_progressBar.SetSmoothProgressDrawableColors(Resources.GetIntArray(Resource.Array.gplus_colors));
		}

		public int DpToPx(int dp) {
			Resources r = Resources;
			int px = (int) TypedValue.ApplyDimension(ComplexUnitType.Dip,dp, r.DisplayMetrics);
			return px;
		}
	}
}

