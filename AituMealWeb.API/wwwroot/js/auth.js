$(document).ready(function () {

$("#signupForm").validate({
    rules: {
      email: {
        required: true,
        email: true
      },
      firstName: "*",
      lastName: "*",
      password: {
        required: true,
        minlength: 6,
        maxlength: 15
      }
    },
    messages: {
      email: "Please, enter valid email",
      firstName: "Please, enter your First Name",
      lastName: "Please, enter your Last Name",
      password: {
        minlength: "Password should be at least 6 characters",
        maxlength: "Password should be no more than 15 characters"
      }
    },
    submitHandler: function(form)
    {
      form.submit();
    }
  });


    $('#regform').submit(function (e) {
        e.preventDefault();
        var formData = $(this).serializeArray();

        var serializedData = toJSONString(formData);
        var url = '/api/auth/register';
        $.ajax({
            url: url,
            type: 'POST',
            data: serializedData,
            contentType: 'application/json',
            success: function () {
                alert('Successfully registered.');
                $('.msg').after('<a href="auth.html">Now you can authorize!</a>');
            },
            error: function (e) {
                console.log(e);
                alert('Some problems occured! Please, enter valid details!\n'+e.responseText);
            }
        });
    });



    $('#loginform').submit(function (e) {
        e.preventDefault();

        var lformData = $(this).serializeArray();

        var serializedDetails = toJSONString(lformData);

        var url = '/api/auth/login';
        $.ajax({
            url: url,
            type: 'POST',
            data: serializedDetails,
            contentType: 'application/json',
            success: function (data) {
                //storing jwt token
                localStorage.setItem('token', data);
                location = '/myprofile.html'
            },
            error: function (e) {
                console.log(e);
                alert(e.responseText);
            }
        });

    });

});
