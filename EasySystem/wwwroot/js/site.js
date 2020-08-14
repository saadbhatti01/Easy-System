// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
//For Notofication
$(document).ready(function () {
    setTimeout(function () {
        $("#Notifocation").fadeOut();
    }, 5000);
});

$(document).ready(function () {

    $('.date-picker').datepicker({
        orientation: "top auto",
        format: "mm-dd-yyyy",
        autoclose: true
    }).datepicker("setDate", new Date());
});


$(document).ready(function () {

    "use strict"; // Start of use strict

    // Footable 
    $('#List').footable();

});

$(document).ready(function () {

    "use strict"; // Start of use strict

    // Footable 
    $('#List1').footable();

});

$(document).ready(function () {

    "use strict"; // Start of use strict

    // Footable 
    $('#List2').footable();

});

$(document).ready(function () {

    "use strict"; // Start of use strict

    // Footable 
    $('#List3').footable();

});




function myFunction() {
    var input, filter, table, tr, td, i;
    input = document.getElementById("Input");
    filter = input.value.toUpperCase();
    table = document.getElementById("List");
    tr = table.getElementsByTagName("tr");
    for (i = 0; i < tr.length; i++) {
        td = tr[i].getElementsByTagName("td")[1];
        if (td) {
            if (td.innerHTML.toUpperCase().indexOf(filter) > -1) {
                tr[i].style.display = "";
            } else {
                tr[i].style.display = "none";
            }
        }
    }
}

function myFunction() {
    var input, filter, table, tr, td, i;
    input = document.getElementById("Input");
    filter = input.value.toUpperCase();
    table = document.getElementById("List1");
    tr = table.getElementsByTagName("tr");
    for (i = 0; i < tr.length; i++) {
        td = tr[i].getElementsByTagName("td")[1];
        if (td) {
            if (td.innerHTML.toUpperCase().indexOf(filter) > -1) {
                tr[i].style.display = "";
            } else {
                tr[i].style.display = "none";
            }
        }
    }
}
