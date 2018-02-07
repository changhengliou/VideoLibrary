import 'bootstrap-table';
import { isEmpty, addMsgbox, tableSetting, setTableViewZhTwLocal, getDateTimeString } from './js/utils';

setTableViewZhTwLocal($);

$('#table').bootstrapTable({
    ...tableSetting,
    showRefresh: false,
    url: `/api/v1/log/user`,
    queryParams: (params) => null,
    columns: [{
        field: 'UserName',
        title: '帳號'
    }, {
        field: 'Action',
        title: '動作'
    }, {
        field: 'ActionTime',
        title: '時間',
        formatter: (value, row, index, field) => getDateTimeString(new Date(value))
    }]
});