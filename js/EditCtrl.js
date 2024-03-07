(function ($) {
    $(function () {
        if ($("#hidCheck").val() == "Y") {
            $("input[name=chkSubscribe]").attr("checked", "checked");
        }
        else {
            $("input[name=chkSubscribe]").removeAttr("checked");
        };
        
    });
})(jQuery);