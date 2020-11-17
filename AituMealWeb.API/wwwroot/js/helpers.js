$(window).on('load', function () {
    if (localStorage.getItem('token') == '') {
        alert('You need to authorize firstly!');
        location = '/auth.html';
    }else{

      $('body').css({'visibility':'visible'});
      let jwt = parseJwt(localStorage.getItem('token'));
      switch (jwt.role) {
        case "Admin":
        $('.navigation').prepend('<a href="/menu.html">Manage Menu Items</a>'+
                               '<a href="#">Manage Meal Categories</a>'+
                               '<a href="#">Manage User Details</a>'+
                               '<a href="#">Manage Order Details</a>' +
                               '<a href="/myprofile.html">Profile</a>');
          break;
        case "Kassir":
        $('.navigation').prepend('<a href="/menu.html">Manage Menu Items</a>'+
                               '<a href="#.html">Manage Incoming Orders</a>'+
                               '<a href="/myprofile.html">Profile</a>');
          break;
        default:
        $('.navigation').prepend('<a href="/menu.html">Observe Menu</a>'+
                               '<a href="#">My Orders</a>'+
                               '<a href="/myprofile.html">Profile</a>');
          break;
      }
    }
});

$('#logout').click(function () {
    logout();
    location = '/auth.html';
});

function logout() {
    localStorage.setItem('token', null);
}


function parseJwt (token) {
    var base64Url = token.split('.')[1];
    var base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    var jsonPayload = decodeURIComponent(atob(base64).split('').map(function(c) {
        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
    }).join(''));

    return JSON.parse(jsonPayload);
};

function toJSONString(form) {
    var obj = {};
    var elements = form;
    for (var i = 0; i < elements.length; ++i) {
        var element = elements[i];
        var name = element.name;
        var value = element.value;

        if (name) {
            obj[name] = value;
        }
    }

    return JSON.stringify(obj);
}
