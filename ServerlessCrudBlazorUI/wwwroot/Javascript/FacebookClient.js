
// Reference: https://developers.facebook.com/docs/reference/javascript/FB.getLoginStatus/

window.FacebookClient = {
    logIn: function (dotnetHelper) {
        FB.init({
            appId: '201848331183252',
            autoLogAppEvents: true,
            xfbml: true,
            version: 'v7.0'
        });

        FB.getLoginStatus(function (response) {
            if (response.status === 'connected') {
                dotnetHelper.invokeMethodAsync('InvokeCallback', [response.authResponse.userID, response.authResponse.accessToken]);
            } else {
                FB.login(function (r) {
                    dotnetHelper.invokeMethodAsync('InvokeCallback', [r.authResponse.userID, r.authResponse.accessToken]);
                });
            }
        });
    }, 
    logOut: function () {
        FB.logout(function (response) {
            
        });
    }
};