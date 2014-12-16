using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Provider;
using Android.Graphics;

namespace CropImageSample
{
	[Activity (Label = "CropImageSample", MainLauncher = true)]
	public class MainActivity : Activity
	{
		const int CAMERA_CAPTURE = 1;
		//keep track of cropping intent
		const int PIC_CROP = 2;
		//captured picture uri
		private Android.Net.Uri picUri;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);
			Button captureBtn = FindViewById<Button> (Resource.Id.capture_btn);// (Button)findViewById(R.id.capture_btn);
			captureBtn.Click += (sender, e) => {
				try {
					//use standard intent to capture an image
					Intent captureIntent = new Intent(MediaStore.ActionImageCapture);
					//we will handle the returned data in onActivityResult
					StartActivityForResult(captureIntent, CAMERA_CAPTURE);
				}
				catch(ActivityNotFoundException anfe){
					//display an error message
					String errorMessage = "Whoops - your device doesn't support capturing images!";
					Toast toast = Toast.MakeText(this, errorMessage, ToastLength.Short);
					toast.Show();
				}
			};
		}
		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult (requestCode, resultCode, data);
			if (resultCode == Result.Ok) {
				//user is returning from capturing an image using the camera
				if(requestCode == CAMERA_CAPTURE){
					//get the Uri for the captured image
					picUri = data.Data;
					//carry out the crop operation
					performCrop();
				}
				//user is returning from cropping the image
				else if(requestCode == PIC_CROP){
					//get the returned data
					Bundle extras = data.Extras;
					//get the cropped bitmap
					Bitmap thePic = (Android.Graphics.Bitmap)extras.GetParcelable("data");
					//retrieve a reference to the ImageView
					ImageView picView = FindViewById<ImageView>(Resource.Id.picture);
					//display the returned cropped image
					picView.SetImageBitmap(thePic);
					//Store Image in Phone
					Android.Provider.MediaStore.Images.Media.InsertImage(ContentResolver,thePic,"imgcrop","Description");
				}
			}
		}

		/**
	     * Helper method to carry out crop operation
	     */
		private void performCrop(){
			//take care of exceptions
			try {
				//call the standard crop action intent (the user device may not support it)
				Intent cropIntent = new Intent("com.android.camera.action.CROP"); 
				//indicate image type and Uri
				cropIntent.SetDataAndType(picUri, "image/*");
				//set crop properties
				cropIntent.PutExtra("crop", "true");
				//indicate aspect of desired crop
				cropIntent.PutExtra("aspectX", 1);
				cropIntent.PutExtra("aspectY", 1);
				//indicate output X and Y
				cropIntent.PutExtra("outputX", 256);
				cropIntent.PutExtra("outputY", 256);
				//retrieve data on return
				cropIntent.PutExtra("return-data", true);
				//start the activity - we handle returning in onActivityResult
				StartActivityForResult(cropIntent, PIC_CROP);  
			}
			//respond to users whose devices do not support the crop action
			catch(ActivityNotFoundException anfe){
				//display an error message
				String errorMessage = "Whoops - your device doesn't support the crop action!";
				Toast toast = Toast.MakeText(this, errorMessage, ToastLength.Short);
				toast.Show();
			}
		}
	}
}


