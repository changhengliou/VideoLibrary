import 'bootstrap-table';
import 'bootstrap-table/dist/bootstrap-table.css';
import 'bootstrap-datepicker/js/bootstrap-datepicker.js';
import 'bootstrap-datepicker/dist/css/bootstrap-datepicker.css';
import { setDatePickerZhTw, setTableViewZhTwLocal, tableSetting, getDateTimeString, getDateString, isEmpty, addMsgbox } from './js/utils';
import './css/publish.css';

const query = /q=(\d{4}-\d{1,2}-\d{1,2})/i.exec(location.search);
const queryDate = query ? query[1] : getDateString();

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

$('#schedule-datepicker').datepicker({
    autoclose: true,
    language: 'zhtw',
})
.val(queryDate)
.on('changeDate', (e) => {
    var tb = $('#schedule-table');
    tb.bootstrapTable('refresh', { url: `/api/v1/schedule/${getDateString(e.date)}` })
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
