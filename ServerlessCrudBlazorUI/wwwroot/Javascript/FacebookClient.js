
// Reference: https://developers.facebook.com/docs/reference/javascript/FB.getLoginStatus/

window.FacebookClient = {
    getAccessToken: function () {
        var uid;
        var accessToken;

        FB.init({
            appId: '201848331183252',
            autoLogAppEvents: true,
            xfbml: true,
            version: 'v7.0'
        });

        FB.getLoginStatus(function (response) {
            if (response.status === 'connected') {
                uid = response.authResponse.userID;
                accessToken = response.authResponse.accessToken;
                console.log(uid);
                console.log(accessToken);
            } else {
                FB.login(function (r) {
                    uid = r.authResponse.userID;
                    accessToken = r.authResponse.accessToken;
                    console.log(uid);
                    console.log(accessToken);
                });
            }
        });
    }
};