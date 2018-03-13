import 'bootstrap-table';
import 'bootstrap-table/dist/bootstrap-table.css';
import 'bootstrap-datepicker/js/bootstrap-datepicker.js';
import 'bootstrap-datepicker/dist/css/bootstrap-datepicker.css';
import {
    setDatePickerZhTw,
    setTableViewZhTwLocal,
    tableSetting,
    getDateTimeString,
    getDateString,
    isEmpty,
    addMsgbox
} from './js/utils';
import './css/publish.css';

const query = /q=(\d{4}-\d{1,2}-\d{1,2})/i.exec(location.search);
const queryDate = query ? query[1] : getDateString();

$(document).ready(() => {
    setDatePickerZhTw($);
    setTableViewZhTwLocal($);
    $('#schedule-table').bootstrapTable({
        ...tableSetting,
        url: `/api/v1/schedule/${queryDate}`,
        queryParams: (params) => {
            return null;
        },
        showRefresh: false,
        pageSize: 10,
        pageList: [10, 20, 50],
        columns: [{
            field: 'Name',
            title: '活動名稱',
        }, {
            field: 'ScheduleDate',
            title: '上架日期',
            formatter: (value) => getDateString(new Date(value)) + " 00:00:00"
        }, {
            field: 'ScheduleDateEnd',
            title: '下架日期',
            formatter: (value) => getDateString(new Date(value)) + " 23:59:59"
        }, {
            field: 'CreateDate',
            title: '創建日期',
            formatter: (value) => getDateTimeString(new Date(value))
        }, {
            field: 'Remove',
            title: '刪除',
            formatter: (value, row, index, field) =>
                `<button class='btn btn-sm btn-default' data-id="${row.Id}" onClick='$.fn.rmSchedule(this);'>
                <span class="glyphicon glyphicon-remove"></span>
            </button>`
        }]
    });

    if (new Date(queryDate) < Date.now())
        $('#schedule-table').bootstrapTable('hideColumn', 'Remove');
    else
        $('#schedule-table').bootstrapTable('showColumn', 'Remove');

    $('#datepicker').datepicker({
        autoclose: true,
        startDate: '+0d',
        language: 'zhtw'
    });
    $('#schedule-datepicker').datepicker({
            autoclose: true,
            language: 'zhtw',
        })
        .val(queryDate)
        .on('changeDate', (e) => {
            var tb = $('#schedule-table');
            tb.bootstrapTable('refresh', {
                url: `/api/v1/schedule/${getDateString(e.date)}`
            })
            if (e.date < Date.now())
                tb.bootstrapTable('hideColumn', 'Remove');
            else
                tb.bootstrapTable('showColumn', 'Remove');
        });


    $.fn.rmSchedule = (e) => {
        var id = e.getAttribute('data-id');
        if (isEmpty(id)) {
            addMsgbox('系統錯誤!', '請嘗試重新整理', 'schedule-msg', 'danger');
            return;
        }
        $.ajax({
                url: `/api/v1/schedule/${id}`,
                method: 'DELETE'
            })
            .done(res => {
                $('#schedule-table').bootstrapTable('removeByUniqueId', id);
                addMsgbox('刪除成功!', null, 'schedule-msg', 'success');
            })
            .fail(err => {
                addMsgbox('刪除失敗!', null, 'schedule-msg', 'danger');
            });
    }
    const onbBtnClick = (e) => {
            if (e.target.id === 'pubbtn') {
                window.open('/api/v1/publish');
                addMsgbox("發布完成", null, "schedule-msg", "success");
            } else if (e.target.id === 'downloadbtn') {
                var _date = $('#schedule-datepicker').val();
                if (isEmpty(_date)) {
                    addMsgbox('日期不能為空!', null, 'schedule-msg', 'danger');
                    return;
                }
                window.open(`/api/v1/publish/download?s=${_date}`);
            } else if (e.target.id === 'excelbtn') {
                $('#calendarModal').modal('toggle');
            } else if (e.target.id === 'calendarUpdate') {
                $('#calendarModal').modal('toggle');
                var from = $('#datepicker').data('datepicker').dates[0],
                    to = $('#datepicker').data('datepicker').dates[1];
                if (isEmpty(from) || isEmpty(to)) {
                    addMsgbox('日期不能為空!', null, 'schedule-msg', 'danger');
                    return;
                }
                window.open(`/api/v1/publish/excelsheet?s=${getDateString(new Date(from))}&e=${getDateString(new Date(to))}`);
            }
        }
        ['pubbtn', 'downloadbtn', 'excelbtn', 'calendarUpdate'].map(fn => {
            document.getElementById(fn).addEventListener('click', onbBtnClick);
        });
});