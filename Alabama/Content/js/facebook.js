window.fbAsyncInit = function () {
    FB.init({
        //appId: '542351795895174', // Set YOUR APP ID
        //channelUrl: 'http://viebooks.com:81/', // Channel File
        appId: '1579793945574099', // Set YOUR APP ID1579793945574099
        channelUrl: 'http://localhost:1516/', // Channel File
        status: true, // check login status
        cookie: true, // enable cookies to allow the server to access the session
        xfbml: true  // parse XFBML
    });
}

function Login() {
    // Login my system viebooks
    FB.login(function (response) {
        if (response.authResponse) {
            getUserInfo();
        } else {
            console.log('User cancelled login or did not fully authorize.');
        }
    }, { scope: 'email,user_photos,user_videos' });
}

function UploadToFanPage(arryBook) {
    FB.login(function (response) {
        if (response.authResponse) {
            var page = null;
            FB.api('/me/accounts', function (response) {
                console.log(response);
                page = response.data[0];
                for (var item in arryBook) {
                    if (arryBook.hasOwnProperty(item)) {
                        postToPage(page, arryBook[item].message, arryBook[item].link, arryBook[item].name, arryBook[item].caption, arryBook[item].description);
                        //photoToPage(page, 'http://24.media.tumblr.com/tumblr_m1ttif5puW1qcrr0lo1_500.png', 'nyan art');
                    }
                }
                new PNotify({
                    title: 'Thông báo',
                    text: 'Cập nhật thành công!',
                    type: 'success',
                    hide: true
                });
            });
        } else {
            console.log('User cancelled login or did not fully authorize.');
        }
    }, { scope: 'email,user_photos,user_videos' });
}

function postToPage(page, msg, link, name, caption, description) {
    FB.api('/' + page.id + '/feed', 'post', {
        message: msg,
        link: link,
        name: name,
        caption: caption,
        description: description,
        access_token: page.access_token
    },
      function (res) { console.log(res) }
    )
}
function photoToPage(page, src, msg) {
    FB.api('/' + page.id + '/photos', 'post', { url: src, message: msg, access_token: page.access_token },
      function (res) { console.log(res) }
    )
}
function getUserInfo() {
    FB.api('/me', function (response) {
        var avata_link = getPhoto(response.id) + "?type=large&width=190&height=190";
        $.ajax({
            type: "GET",
            url: getFacbookLoginURL(),
            data: { 'fb_id': response.id, 'userName': response.username, 'name': response.name, 'email': response.email, 'avata_link': avata_link },
            success: function (dataCheck) {
                location.href = dataCheck;
            }
        });
    });
}
function getPhoto(id) {
    var xxx = "https://graph.facebook.com/" + id + "/picture";
    return xxx;
}

function Logout() {
    FB.logout(function () { document.location.reload(); });
}

// Load the SDK asynchronously
(function (d) {
    var js, id = 'facebook-jssdk', ref = d.getElementsByTagName('script')[0];
    if (d.getElementById(id)) { return; }
    js = d.createElement('script'); js.id = id; js.async = true;
    js.src = "//connect.facebook.net/vi_VN/all.js";
    ref.parentNode.insertBefore(js, ref);
} (document));