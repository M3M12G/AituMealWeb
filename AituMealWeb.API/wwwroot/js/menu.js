$(document).ready(function () {

  //By default, todays date
  getMenu('api/menu');

  //used to show single menu item
   $('body').off('click').on('click','img',function(){
      getMenuOnImg(this);
   });


$('#xBtn').click(function () {
  $('.menuItem').css({'display':'none'});
});

    async function getMenu(urlwithDate) {

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
                    var chunked = data[0];
                    var menuFor = chunked.menuFor;
                    let mealsByTime = chunked.mealsByTime;
                    $('.content').prepend('<h3>'+(new Date(menuFor).toDateString())+'</h3>');
                    //console.log(mealsByTime);
                      let tableWithDt = '';

                    for(let i=0; i<mealsByTime.length; i++){//diving by day time
                        let dt = mealsByTime[i].dayTime;
                        let meals = mealsByTime[i].mealOnMenu;

                        let mealRow = '';

                              for(let j=0; j<meals.length; j++){
                                mealRow += '<tr>';
                                  for(ind in meals[j]){//reading meal details
                                    if(ind === 'id' | ind === 'mealId' | ind === 'mealMealCategoryId'){
                                      mealRow = mealRow + '<td id='+ind+' hidden>'+meals[j][ind]+'</td>';
                                    } else {
                                          if(ind ==='mealPicture'){
                                              mealRow = mealRow + '<td id='+ind+'><img class="pict" src='+meals[j][ind]+' alt="'+meals[j]['id']+'"></td>';
                                          }else{
                                              mealRow = mealRow + '<td id='+ind+'>'+meals[j][ind]+'</td>';
                                          }
                                    }

                                  }
                                  mealRow = mealRow + '</tr>';
                              }

                          tableWithDt = tableWithDt+'<tr>'
                                        +whichDayTime(dt)+
                                            +'</tr>' + mealRow;

                    }
                    $('table.menu').append(tableWithDt);
                    $('table.menu').append('</table>');
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


function whichDayTime(dayTimeNum){
  switch (dayTimeNum) {
    case "1":
        return '<th colspan="6">Breakfast</th>';
      break;
    case "2":
        return '<th colspan="6">Lunch</th>';
        break;
    case "3":
        return '<th colspan="6">Dinner</th>';
        break;
    default:
      return '<th colspan="6">lunch</th>';
      break;
  }
}


async function getMenuOnImg(img){
    let id = img.alt;
    url = 'api/menu/'+id;


    let response = await fetch(url, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json;charset=utf-8',
            'Authorization': 'Bearer ' + localStorage.getItem('token')
        }
    });

    if (response.ok) {

          response.json().then(function (data) {
              $('#itemImg').attr('src', data.mealPicture);
              $('#itemId').text(data.id);
              $('#itemMealId').text(data.mealId);
              $('#itemName').text(data.mealName);
              $('#itemMAmount').text(data.mealAmount);
              $('#itemMPrice').text(data.mealPrice);
              $('#itemMCatId').text(data.mealMealCategoryId);
              $('#itemMCat').text(data.mealMealCategoryCategory);
              $('#itemStock').text(data.inStock);
              $('.menuItem').css({'display':'block'});
          });

    }else{
      switch (response.status) {
        case 401:
          alert('You should authorize first!');
          location = '/auth.html';
          break;
        case 404:
          alert('Menu with such ID is not found!');
          break;
        default:
          alert('Some unwanted issues appeared');
          break;
                              }
          }

  }

});
