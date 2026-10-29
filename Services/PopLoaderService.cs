using AttendEdgeMobileClient.PopupPages;
using Mopups.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendEdgeMobileClient.Services;

public static class PopLoaderService
{
    private static CustomLoaderPopup? _loaderPopup;
    private static bool _isShowing;


    public static async Task ShowAsync()
    {
        if (_isShowing)
            return;

        _isShowing = true;

        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            _loaderPopup = new CustomLoaderPopup();

            await MopupService.Instance.PushAsync(_loaderPopup);
        });
    }



    public static async Task HideAsync()
    {
        if (!_isShowing || _loaderPopup == null)
            return;

        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await MopupService.Instance.RemovePageAsync(_loaderPopup);

            _loaderPopup = null;
            _isShowing = false;
        });
    }
}
