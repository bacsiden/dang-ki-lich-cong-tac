$(document).ready(function () {
    // thêm thẻ <li> cho phân trang    
    addClassForPagging();
    $(".confirmDelete").click(function () {
        if (confirm("Bạn có chắc chắn?")) {
            return true;
        }
        return false;
    });
    $("#SubmitRole").click(function () {
        var idlist = "";
        $("#ListRoles input:checkbox:checked").each(function () {
            idlist += $(this).val() + ",";
        });
        $("#HiddenListRole").val(idlist);
    })
    $(".IsActive").click(function () {
        var $this = $(this);
        var id = $this.attr("getID");
        $.ajax({
            type: "post",
            url: "/Menu/DoActive",
            data: "id=" + id,
            success: function (kq) {
                $this.html(kq);
                location.href = "";
            }
        });
    });
    // ------------------------------------------------------------------

    // show hide button Edit & Delete when click checkbox
    $(".checkAll").click(function () {
        $(".EditItem").hide();
        if ($(this).is(':checked')) {
            $(".checkitem").prop('checked', true);
            $(".DeleteItem").show();
            $(".LockUser").show();
        } else {
            $(".checkitem").prop('checked', false);
            $(".DeleteItem").hide();
            $(".LockUser").hide();
        }
    });
    $(".checkitem").click(function () {
        $(".DeleteItem").show();
        $(".LockUser").show();
        var numberOfChecked = $('input:checkbox:checked').length;
        var numberItem = $('input:checkbox.checkitem').length;
        if (numberOfChecked == 0) {
            $(".DeleteItem").hide();
            $(".LockUser").hide();
        }
        if (numberOfChecked == 1) {
            $(".EditItem").show();
        } else {
            $(".EditItem").hide();
        }
        if ($(this).is(':checked')) {
            if (numberItem == numberOfChecked) {
                $(".checkAll").prop('checked', true);
            }
        } else {
            $(".checkAll").prop('checked', false);
        }

    });


    // Event click with button Edit & Delete
    $(".DeleteItem").click(function () {
        var listID = "";
        var url = $(this).attr('href');
        $('input.checkitem:checkbox:checked').each(function () {
            listID += "," + $(this).val();
        });
        if (listID != "") {
            $(this).attr('href', url + "?arrayID=" + listID);
        } else {
            alert('Warring!');
            return false;
        }
    });

    $(".LockUser").click(function () {
        var listID = "";
        var url = $(this).attr('href');
        $('input.checkitem:checkbox:checked').each(function () {
            listID += "," + $(this).val();
        });
        if (listID != "") {
            $(this).attr('href', url + "?arrayID=" + listID);
        } else {
            alert('Warring!');
            return false;
        }
    });

    $(".EditItem").click(function () {
        var obj = $('input.checkitem:checkbox:checked');
        if (obj.length == 1) {
            var value = obj.val();
            var url = $(this).attr('href');
            $(this).attr('href', url + "/" + value);
        } else {
            alert('Warring!');
            return false;
        }
    });
    // ------------------------------------------------------------------

    // Enter for Custom info input text
    $('input.Customer').bind("enterKey", function (e) {
        var query = $(this).val();
        $.ajax({
            type: "GET",
            url: "/TripInfo/GetCutomerInfoByKey",
            data: { "query": query },
            success: function (kq) {
                $(".resultCustomer").html(kq);
            }
        });
    });
    $('input.Customer').keyup(function (e) {
        if (e.keyCode == 13) {
            $(this).trigger("enterKey");
        }
    });
    // ------------------------------------------------------------------

    // ---------- Choice Customer Info in the future----------------------

    $("#Ajax").on("click", ".choiceCustomer", function (event) {
        // Fill data when click Customer info item
        var $this = $(this);
        var name = $.trim($this.parent().next().text());
        var street = $.trim($this.parent().next().next().text());
        var city = $.trim($this.parent().next().next().next().text());
        var state = $.trim($this.parent().next().next().next().next().text());
        var phone = $.trim($this.parent().next().next().next().next().next().text());
        var zip_code = $.trim($this.parent().next().next().next().next().next().next().text());
        var adress = street + ", " + city + ", " + state + " " + zip_code + ", Phone: " + phone;
        $("#Customer").val($this.text());
        $(".CustomerDisplay").val(name);
        $("#Address").val(adress);

        // hide Modal
        $("#AddCustomerInfo").modal('hide');
    });
    //-----------------------------------------------------------------------

    // Focus input text first when modal show
    $('#AddCustomerInfo').on('shown.bs.modal', function () {
        $("input.Customer").focus();
    })
    //-----------------------------------------------------------------------

    // AutoComplete Chosen select item
    $(".chosen-with-diselect-trip").chosen({ allow_single_deselect: true }).change(function () {
        var valueTripInfo = $(".selectTripInfo").val();
        var valueDriverInfo = $(".selectDriverInfoTrip").val();
        $.ajax({
            type: "GET",
            url: '/TripInfo/Index',
            data: { 'tripID': valueTripInfo, 'driverID': valueDriverInfo },
            async: true,
            success: function (model) {
                $("#wrap-AjaxPaging").html(model);
                addClassForPagging();
            }
        });
    });

    // Not EDIT OR DELETE
    $(".chosen-with-diselect").chosen({ allow_single_deselect: true }).change(function () {
        var valueDriverInfo = $(this).val();
        var url = $(location).attr('href');
        $.ajax({
            type: "GET",
            url: url,
            data: { 'driverID': valueDriverInfo },
            async: true,
            success: function (model) {
                $("#wrap-AjaxPaging").html(model);
                addClassForPagging();
            }
        });
    });
    //--------------------------------------------------

    // Report--------------------------
    $(".ReportMenu .AllDate").click(function () {
        if ($(this).is(":checked")) {
            $(".StartDate").attr('Disabled', 'true');
            $(".EndDate").attr('Disabled', 'true');
        } else {
            $(".StartDate").removeAttr('Disabled');
            $(".EndDate").removeAttr('Disabled');
        }
    });
    $(".report a:not(.report-fieldset)").click(function () {
        var isAllDate = $(".AllDate").is(":checked");
        var startDate = $(".StartDate").val();
        var endDate = $(".EndDate").val();
        var selectDispatcher = $(".SelectDispatcher").val();
        var selectDriver = $(".SelectDriver").val();
        var url = $(this).attr('href');
        var newurl = addParam(addParam(addParam(addParam(url, "startDate", startDate), "endDate", endDate), "selectDriver", selectDriver), "selectDispatcher", selectDispatcher);
        if (isAllDate) {
            newurl = addParam(addParam(addParam(url, "allDate", true), "selectDriver", selectDriver), "selectDispatcher", selectDispatcher);
        }
        window.open(newurl, '_blank');
        return false;
    });

    //--------------------------------


});               //----------------END DOCUMENT READY FUNCTION -------------------------------  



// autocomplete 
function autoCompleteByClassName(className1, dataJson) {
    className1.typeahead({
        source: function (query, process) {
            objects = [];
            map = {};
            var data = dataJson // Or get your JSON dynamically and load it into this variable
            $.each(data, function (i, object) {
                map[object.label] = object;
                objects.push(object.label);
            });
            process(objects);
        },
        updater: function (item) {
            this.$element.attr('value', map[item].id)
            return item;
        }
    });
}

// ------------------------------------------------------------------
function addClassForPagging() {
    $(".phantrangmvcpager a").wrap("<li></li>");
    $(".phantrangmvcpager li.active").wrapInner("<a href='#' onclick='return false;'></a>");
    $(".phantrangmvcpager a[disabled='disabled']").parent().addClass('disabled');
}

function ajaxPagingByID(wrapID) {

    $(".phantrangmvcpager a[data-ajax='true']").click(function () {
        var url = $(this).attr('href');
        $.ajax({
            type: "GET",
            url: url,
            async: true,
            success: function (model) {
                $("#" + wrapID).html(model);
                addClassForPagging();
                return false;
            }
        });
        return false;
    });
}

function addParam(url, param, value) {
    var a = document.createElement('a'), regex = /[?&]([^=]+)=([^&]*)/g;
    var match, str = []; a.href = url; value = value || "";
    while (match = regex.exec(a.search))
        if (encodeURIComponent(param) != match[1]) str.push(match[1] + "=" + match[2]);
    str.push(encodeURIComponent(param) + "=" + encodeURIComponent(value));
    a.search = (a.search.substring(0, 1) == "?" ? "" : "?") + str.join("&");
    return a.href;
}
function getListIDCheckBoxByClassName(classWrapper, classItem) {
    var listID = "";
    $('.' + classWrapper + ' input.' + classItem + ':checkbox:checked').each(function () {
        listID += "," + $(this).val();
    });
    return listID;
}

function convertDateTimeVItoEN(date) {
    var arr = date.split('/');
    return arr[1] + "/" + arr[0] + "/" + arr[2] + " ";
}