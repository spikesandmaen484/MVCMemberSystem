(function ($) {
    $(function () {
        if ($("#hidCheck").val() == "Update") {
            $("#btn").val("­×§ï");
        }
        else {
            $("#btn").val("·s¼W");
        };

        $("#selMember").append($("#hidStr").val());
    });
})(jQuery);