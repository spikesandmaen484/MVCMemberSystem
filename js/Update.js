(function ($) {
    $(function () {
        if ($("#hidCheck").val() == "Update") {
            $("#btn").val("�ק�");
        }
        else {
            $("#btn").val("�s�W");
        };

        $("#selMember").append($("#hidStr").val());
    });
})(jQuery);