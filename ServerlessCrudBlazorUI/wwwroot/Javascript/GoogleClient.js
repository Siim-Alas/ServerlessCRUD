
// Reference: https://developers.google.com/identity/sign-in/web/reference

window.GoogleClient = {
    logIn: function (dotnetHelper) {
        gapi.load('auth2', function () {
            gapi.auth2.init({ client_id: '704066166279-dunk2o8blaedb7l149qnu5mphsm1jo69.apps.googleusercontent.com' })
            .then(function (googleAuth) {
                let user;
                let response;

                if (googleAuth.isSignedIn.get()) {
                    // The user is already signed in.
                    user = googleAuth.currentUser.get();

                    dotnetHelper.invokeMethodAsync(
                        'InvokeCallback',
                        [
                            user.getBasicProfile().getName(),
                            user.getId(),
                            user.getAuthResponse().id_token
                        ]);
                } else {
                    googleAuth.signIn()
                    .then(function (googleUser) {
                        // The user was signed in.
                        response = googleUser.getAuthResponse();

                        dotnetHelper.invokeMethodAsync(
                            'InvokeCallback',
                            [
                                googleUser.getBasicProfile().getName(),
                                googleUser.getId(),
                                googleUser.getAuthResponse().id_token
                            ]);
                    }, function () {
                        // Something went wrong signing the user in.
                    });
                }
            }, function () {
                // Something went wrong.
            });
        });
    }, 
    logOut: function () {
        gapi.auth2.getAuthInstance().signOut();
    }
};