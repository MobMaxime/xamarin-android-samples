using System;
using Android.Widget;
using Android.Util;
using Android.Content;
using Android.Views;
using Android.Content.Res;
using Android.Graphics;
using Android.Text;
using Android.Views.Animations;
using Android.Animation;
using Android.Runtime;

namespace LibraryFloatLable
{
	public class FloatLabelEditText : LinearLayout,ITextWatcher,Android.Views.View.IOnFocusChangeListener
	{
		private int _currentApiVersion = (int)Android.OS.Build.VERSION.SdkInt, _focusedColor, _fitScreenWidth, _gravity;
		Color _unFocusedColor;
		private float _textSizeInSp;
		private string _hintText, _editText;
		private bool _isPassword = false;

		private IAttributeSet _attrs;
		private Context _context;
		private EditText _editTextView;
		private TextView _txtFloatingLabel;
		ValueAnimator _focusToUnfocusAnimation,_unfocusToFocusAnimation;


		public Color UnFocusedColor
		{
			get{
				return this._unFocusedColor;
			}
			set {
				this._unFocusedColor = value;
				Invalidate ();
			}
		}


		public float TextSizeInSp
		{
			get{
				return this._textSizeInSp;
			}
			set {
				this._textSizeInSp = value;
				Invalidate ();
			}
		}

		public string HintText
		{
			get{
				return this._hintText;
			}
			set {
				this._hintText = value;
				Invalidate ();
			}
		}

		public string EditText
		{
			get{
				return this._editText;
			}
			set {
				this._editText = value;
				Invalidate ();
			}
		}
		public IAttributeSet Attrs
		{
			get{
				return this._attrs;
			}
			set {
				this._attrs = value;
				Invalidate ();
			}
		}

		public EditText EditTextView
		{
			get{
				return this._editTextView;
			}
			set {
				this._editTextView = value;
				Invalidate ();
			}
		}
		// -----------------------------------------------------------------------
		// default constructors

		public FloatLabelEditText(Context Context):base(Context) {
			_context = Context;
			InitializeView();
		}

		public FloatLabelEditText(Context Context, IAttributeSet Attrs):base(Context, Attrs) {
			_context = Context;
			this.Attrs = Attrs;
			InitializeView();
		}

		public FloatLabelEditText(Context Context, IAttributeSet Attrs, int DefStyle):base(Context, Attrs, DefStyle) {
			_context = Context;
			this.Attrs = Attrs;
			InitializeView();
		}

		// -----------------------------------------------------------------------
		// public interface

		public string GetText() {
			if (GetEditTextString() != null && GetEditTextString().ToString() != null && GetEditTextString().ToString().Length > 0) {
				return GetEditTextString().ToString();
			}
			return "";
		}

		public void SetHint(string HintText) {
			this.HintText = HintText;
			_txtFloatingLabel.Text=HintText;
			SetupEditTextView();
		}

		// -----------------------------------------------------------------------
		// private helpers

		private void InitializeView() {

			if (Context == null) {
				return;
			}

			LayoutInflater Inflater = (LayoutInflater) Context.GetSystemService(Context.LayoutInflaterService);
			Inflater.Inflate(Resource.Layout.FloatLabelEditText, this, true);

			_txtFloatingLabel = FindViewById<TextView>(Resource.Id.floating_label_hint);
			_editTextView = FindViewById<EditText>(Resource.Id.floating_label_edit_text);

			GetAttributesFromXmlAndStoreLocally();
			SetupEditTextView();
			SetupFloatingLabel();
		}

		private void GetAttributesFromXmlAndStoreLocally() {
			TypedArray AttributesFromXmlLayout = Context.ObtainStyledAttributes(Attrs,Resource.Styleable.FloatLabelEditText);
			if (AttributesFromXmlLayout == null) {
				return;
			}

			HintText = AttributesFromXmlLayout.GetString(Resource.Styleable.FloatLabelEditText_hint);
			EditText = AttributesFromXmlLayout.GetString(Resource.Styleable.FloatLabelEditText_text);
			_gravity = AttributesFromXmlLayout.GetInt(Resource.Styleable.FloatLabelEditText_gravity,(int)GravityFlags.Left);
			TextSizeInSp = GetScaledFontSize(AttributesFromXmlLayout.GetDimensionPixelSize(Resource.Styleable.FloatLabelEditText_textSize,(int) _editTextView.TextSize));
			_focusedColor = AttributesFromXmlLayout.GetColor(Resource.Styleable.FloatLabelEditText_textColorHintFocused,Android.Resource.Color.Black);
			UnFocusedColor = AttributesFromXmlLayout.GetColor(Resource.Styleable.FloatLabelEditText_textColorHintUnFocused,Android.Resource.Color.DarkerGray);
			_fitScreenWidth = AttributesFromXmlLayout.GetInt(Resource.Styleable.FloatLabelEditText_fitScreenWidth,0);
			_isPassword = (AttributesFromXmlLayout.GetInt(Resource.Styleable.FloatLabelEditText_inputType,0) == 1);
			AttributesFromXmlLayout.Recycle();
		}

		private void SetupEditTextView() {

			if (_isPassword) {
				_editTextView.InputType = Android.Text.InputTypes.ClassText | Android.Text.InputTypes.TextVariationPassword;
				_editTextView.Typeface = Android.Graphics.Typeface.Default;
			}

			_editTextView.Hint=HintText;
			_editTextView.SetHintTextColor(UnFocusedColor);
			_editTextView.Text=EditText;
			_editTextView.SetTextSize(ComplexUnitType.Sp, TextSizeInSp);
			_editTextView.AddTextChangedListener(this);

			if (_fitScreenWidth > 0) {
				_editTextView.SetWidth(GetSpecialWidth());
			}
			if (_currentApiVersion >= (int)Android.OS.BuildVersionCodes.Honeycomb) {
				_editTextView.OnFocusChangeListener=this;
			}
		}

		private void SetupFloatingLabel() {
			_txtFloatingLabel.Text=HintText;
			_txtFloatingLabel.SetTextColor(UnFocusedColor);
			_txtFloatingLabel.SetTextSize(ComplexUnitType.Sp, (float) (TextSizeInSp / 1.3));
			_txtFloatingLabel.Gravity=GravityFlags.Left;
			_txtFloatingLabel.SetPadding(_editTextView.PaddingLeft, 0, 0, 0);

			if (GetText().Length > 0) {
				ShowFloatingLabel();
			}
		}
		private void ShowFloatingLabel() {
			_txtFloatingLabel.Visibility=ViewStates.Visible;
			_txtFloatingLabel.StartAnimation(AnimationUtils.LoadAnimation(Context,Resource.Animation.weddingparty_floatlabel_slide_from_bottom));
		}
		private void HideFloatingLabel() {
			_txtFloatingLabel.Visibility = ViewStates.Invisible;
			_txtFloatingLabel.StartAnimation(AnimationUtils.LoadAnimation(Context,Resource.Animation.weddingparty_floatlabel_slide_to_bottom));
		}


		void ITextWatcher.AfterTextChanged (IEditable s)
		{
			if (s.Length() > 0 && _txtFloatingLabel.Visibility == ViewStates.Invisible) {
				ShowFloatingLabel();
			} else if (s.Length() == 0 && _txtFloatingLabel.Visibility ==  ViewStates.Visible) {
				HideFloatingLabel();
			}
		}

		private ValueAnimator GetFocusToUnfocusAnimation() {
			if (_focusToUnfocusAnimation == null) {
				_focusToUnfocusAnimation = GetFocusAnimation(UnFocusedColor, _focusedColor);
			}
			return _focusToUnfocusAnimation;
		}

		private ValueAnimator GetUnfocusToFocusAnimation() {
			if (_unfocusToFocusAnimation == null) {
				_unfocusToFocusAnimation = GetFocusAnimation(_focusedColor, UnFocusedColor);
			}
			return _unfocusToFocusAnimation;
		}

		private ValueAnimator GetFocusAnimation(int FromColor, int ToColor) {
			ValueAnimator ColorAnimation = ValueAnimator.OfObject(new ArgbEvaluator(),FromColor,ToColor);
			return ColorAnimation;
		}
		private string GetEditTextString() {
			return _editTextView.Text;
		}

		private float GetScaledFontSize(float FontSizeFromAttributes) {
			float ScaledDensity = Context.Resources.DisplayMetrics.ScaledDensity;
			return FontSizeFromAttributes / ScaledDensity;
		}
		private int GetSpecialWidth() {
			float ScreenWidth = (Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>()).DefaultDisplay.Width;
			int PrevWidth = _editTextView.Width;

			switch (_fitScreenWidth) {
			case 2:
				return (int) Math.Round(ScreenWidth * 0.5);
			default:
				return (int)Math.Round(ScreenWidth);
			}
		}
		void ITextWatcher.BeforeTextChanged (Java.Lang.ICharSequence s, int start, int count, int after)
		{
//			throw new NotImplementedException ();
		}
		void ITextWatcher.OnTextChanged (Java.Lang.ICharSequence s, int start, int before, int count)
		{
//			throw new NotImplementedException ();
		}

		void IOnFocusChangeListener.OnFocusChange (View v, bool hasFocus)
		{
			ValueAnimator lColorAnimation;

			if (hasFocus) {
				_txtFloatingLabel.SetTextColor (Resources.GetColor (Resource.Color.holo_blue_dark));
				lColorAnimation = GetFocusToUnfocusAnimation();
			} else {
				_txtFloatingLabel.SetTextColor (Color.DarkGray);
				lColorAnimation = GetUnfocusToFocusAnimation();
			}

			lColorAnimation.SetDuration(700);
			lColorAnimation.Start();
		}
	}
}

