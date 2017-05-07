//Отображение модального окна со списком связанных персон и средств связи
//Представление ShowPersonCard контроллер PersonCard

$('body').on('click', '.showRelPersons', function (event) {
    event.preventDefault();
    $.ajaxSetup({ cache: false });
    $('#loadingImg').show();
    $.get('/PersonCard/ShowPersonCard', {
        PersonId: $(this).data('personid')
    },
         function (data) {
             $('#dialogContent').html(data);
             $('#modDialog').modal('show');
             $('.comm').first().collapse('show');
             $('#loadingImg').hide();
         });
});

//Отображение средств связи связанных персон
$("body").on('show.bs.collapse', '.comm', function () {
    AjaxLoadComm($(this).attr('id'));
});


$('body').on('hide.bs.modal', '#modDialog', function () {
    saveNote();
})



function AjaxLoadComm(_id) {
    $('#loadingImg').show();
    $('#' + _id + '>div').delay(100).queue(function (nxt) {
        $('#' + _id + '>div').load('/PersonCard/AjaxPersonComm', { PersonId: _id }, function () {
            $('#loadingImg').hide();
        });
        nxt();
    });
};

function saveNote() {
    var _note = $('#PersonNote').val();
    var _personId = $('#PersonNote').data('personid');
    $('#loadingImg').show();
    $.ajax({
        type: 'POST',
        traditional: true,
        url: '/PersonCard/AjaxUpdateInfoes/',
        data: {
            PersonId: _personId,
            Infoes: _note
        },
        success: function (data) {
            $('#loadingImg').hide();
            if (data.Result) {
                //Обновляем грид
                $('#PersonNote').css('background-color', '#DDFFDD');
            }
            else {
                //Операция не удалась
                $('#PersonNote').css('background-color', '#FFDDDD');
            }

        },
        fail: function (data) {
            //Операция не удалась
            $('#loadingImg').hide();
            $('#PersonNote').css('background-color', '#FFDDDD');
        }
    });
}

$('body').on('click', '.hidebtn', function (e) {
    e.preventDefault();
    $(this).blur();
    var _href = this.href;
    $('#modDialog').modal('hide').delay(500).queue(function () {
        window.open(_href, '_blank');
        $(this).dequeue();
    });

});