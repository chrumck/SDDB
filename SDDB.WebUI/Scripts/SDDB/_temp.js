/// <reference path="../DataTables/jquery.dataTables.js" />
/// <reference path="../modernizr-2.8.3.js" />
/// <reference path="../bootstrap.js" />
/// <reference path="../BootstrapToggle/bootstrap-toggle.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../jquery-2.1.4.intellisense.js" />
/// <reference path="../MagicSuggest/magicsuggest.js" />
/// <reference path="Shared.js" />
/// <reference path="Shared_Views.js" />


function myfunction() {
    return testFunctionHelper();

    function testFunctionHelper() {
        return 5;
    }
}

function masterFunction() {
    myfuncti
}

function say667() {
    // Local variable that ends up within closure
    var num = 666;
    var sayAlert = function () { alert(num); }
    num++;
    return sayAlert;
}