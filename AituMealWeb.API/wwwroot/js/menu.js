$(document).ready(function () {

    getMenu();

    //representAsTable();

    async function getMenu() {

        urlwithDate = "api/menu/2020-11-4";

        let response = await fetch(urlwithDate, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json;charset=utf-8',
                'Authorization': 'Bearer ' + localStorage.getItem('token')
            }
        });

        if (response.ok) {
            response.json().then(function (data) {
                if (data.length <= 0) {
                    $('.content').html('<p>No records is available for today</p>');
                } else {
                    //representAsTable(data)
                    var chunked = data[0];
                    var menuFor = chunked.menuFor;

                    representAsTable();

                    $('#menudate').append(new Date(menuFor).toDateString());

                    console.log("Menu For : "+menuFor+"\n");

                    let mealsbyTime = chunked.mealsByTime;

                    for(let i=0; i<mealsbyTime.length; i++){

                      console.log("Length of meals on menu = "+mealsbyTime[i].mealOnMenu.length);
                      for(let j=0; j<mealsbyTime[i].mealOnMenu.length; j++){
                        let row = '<tr>';

                          for(ind in mealsbyTime[i].mealOnMenu[j]){
                              let mealDetail = mealsbyTime[i].mealOnMenu[j][ind];
                              console.log(ind+":"+mealDetail);


                              if(ind === "id" | ind === "mealId" | ind ==="mealMealCategoryId"){
                                row = row + '<td id='+ind+' hidden>'+mealDetail+'</td>';
                              } else{
                                  if(ind =="mealPicture"){
                                      row = row + '<td id='+ind+'><img class="pict" src='+mealDetail+' alt="meal picture"></td>';
                                  } else{
                                      row = row + '<td id='+ind+'>'+mealDetail+'</td>';
                                  }
                              }

                          }
                          row = row + '</tr>';
                          $('tbody').append(row);
                      }
                    }

                }

            });
        } else {
            if (response.status == 401) {
                alert('You should authorize first!');
                location = '/auth.html';
            } else {
                alert('Some unwanted problems occured!');
            }
        }

    }


function representAsTable(){
  $('.content').append(

    '<table>'+
    '<tr>'
   	  +'<th>Menu For</th>'
        +'<th colspan="5" id="menudate"></th>'
    +'</tr>'
  );

  $('table').append(
    '<tr><!--o-->'
    	+'<tr id="daytime">'

        +'</tr>'
        +'<tr>'
            	+'<tr>'
                	+'<th hidden>Menu ID</th>'
                	+'<th hidden>Meal ID</th>'
                  +'<th>Meal Name</th>'
                  +'<th>Picture</th>'
                  +'<th>Amount</th>'
                  +'<th>Price</th>'
                  +'<th hidden>Meal Category ID</th>'
                  +'<th>Meal Category</th>'
                  +'<th>In Stock</th>'
              +'</tr>'

  );
}


    });


/*
switch (mealsbyTime[i].dayTime) {
  case "1":
      console.log('Breakfast');
      $('#daytime').append('<th colspan="6">Breakfast</th>');
    break;
  case "2":
      console.log('Lunch');
      $('#daytime').append('<th colspan="6">Lunch</th>');
    break;
  case "3":
      console.log('dinner');
      $('#daytime').append('<th colspan="6">Dinner</th>');
    break;
  default:
      console.log('lunch');
      $('#daytime').append('<th colspan="6">Lunch</th>');
}
*/
