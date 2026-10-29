using AttendEdgeMobileClient.CustomeControls;
using Microsoft.Maui.Handlers;
#if ANDROID 
using Android.Widget;
#elif IOS
using UIKit;
#endif

namespace AttendEdgeMobileClient.Handlers
{
    public static class BoderlessEntryHandler
    {
        public static void RemoveBorder()
        {
            EntryHandler.Mapper.AppendToMapping(nameof(BorderlessEntry), (handler, view) =>
            {
#if ANDROID
                if (view is BorderlessEntry && handler.PlatformView is EditText editText)
                {
                    // Remove background/border
                    editText.Background = null;
                    editText.SetBackgroundColor(Android.Graphics.Color.Transparent);
                }
#elif IOS
                if (view is BorderlessEntry && handler.PlatformView is UITextField uiTextField)
                {
                    uiTextField.BorderStyle = UITextBorderStyle.None;
                }
#endif
            });

            DatePickerHandler.Mapper.AppendToMapping(nameof(BorderlessDatePicker), (handler, view) =>
            {
#if ANDROID
                if (view is BorderlessDatePicker && handler.PlatformView is EditText editText)
                {
                    // Remove background/border
                    editText.Background = null;
                    editText.SetBackgroundColor(Android.Graphics.Color.Transparent);
                }
#elif IOS
                if (view is BorderlessDatePicker && handler.PlatformView is UITextField uiTextField)
                {
                    uiTextField.BorderStyle = UITextBorderStyle.None;
                }
#endif
            });

            EditorHandler.Mapper.AppendToMapping(nameof(BorderlessEditor), (handler, view) =>
            {
#if ANDROID
                if (view is BorderlessEditor && handler.PlatformView is EditText editText)
                {
                    // Remove border/background
                    editText.Background = null;
                    editText.SetBackgroundColor(Android.Graphics.Color.Transparent);
                }
#elif IOS
                if (view is BorderlessEditor && handler.PlatformView is UITextView uiTextView)
                {
                    uiTextView.BackgroundColor = UIKit.UIColor.Clear;
                    uiTextView.Layer.BorderWidth = 0;
                }
#endif
            });


            PickerHandler.Mapper.AppendToMapping(nameof(BorderlessPicker), (handler, view) =>
            {
#if ANDROID
                if (view is BorderlessPicker && handler.PlatformView is EditText editText)
                {
                    editText.Background = null;
                    editText.SetBackgroundColor(Android.Graphics.Color.Transparent);
                    editText.BackgroundTintList =
                        Android.Content.Res.ColorStateList.ValueOf(
                            Android.Graphics.Color.Transparent);
                }
#elif IOS
                if (view is BorderlessPicker && handler.PlatformView is UITextField uiTextField)
                {
                    uiTextField.BorderStyle = UITextBorderStyle.None;
                    uiTextField.BackgroundColor = UIColor.Clear;
                }
#endif
            });
        }
    }
}