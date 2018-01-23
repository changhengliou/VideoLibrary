// import React from 'react';
// import ReactDom from 'react-dom';
import 'bootstrap-table';
import 'bootstrap-table/dist/bootstrap-table.css';
import 'select2';
import 'select2/dist/css/select2.css'
import { setTableViewZhTwLocal, isEmpty } from './js/utils';

$(document).ready(() => {
    var table = $('#table');
    setTableViewZhTwLocal($);
    table.bootstrapTable({
        onClickCell: (field, value, row, element) => {
            if (field !== 'Edit')
                return;
        },
        url: '/api/v1/ad/events/',
        uniqueId: 'Id',
        iconSize: 'sm',
        locale: 'zh-TW',
        striped: true,
        pagination: true,
        pageNumber: 1,
        pageSize: 10,
        pageList: [10, 25, 50],
        search: false,
        showHeader: true,
        showFooter: false,
        showRefresh: true,
        showToggle: false, //switch between cardView / detailView
        showPaginationSwitch: false, // show/hide pagination
        cardView: false, // if true, switch to card view
        detailView: false, // if true, show plus sign with detail enabled
        rowStyle: (row, index) => { return { css: { "vertical-align": "middle" } } },
        columns: [{
            field: 'Name',
            title: '名稱'
        }, {
            field: 'Edit',
            title: '編輯',
            formatter: (value, row, index, field) => `<button class='btn btn-sm btn-default' data-id="${row.Id}" onClick='$.fn.switchView();'>
                                                          <span class="glyphicon glyphicon-pencil"></span>
                                                      </button>`
        }]
    });   
    $('#locationSelect').select2({
        width: window.innerWidth > 700 ? '60%' : '90%'
    });
    $('#soSelect').select2({
        width: window.innerWidth > 700 ? '60%' : '90%'
    });

    /**
     * Switch view between edit and view panel
     */
    $.fn.switchView = (e) => {
        const edit = document.getElementById('editPanel'),
              view = document.getElementById('viewPanel');
        if (edit.style.display === '') {
            edit.style.display = 'none';
            view.style.display = '';
        } else {
            edit.style.display = '';
            view.style.display = 'none';
        }
    }

    // add event listener to go back button
    document.getElementById('goBack').addEventListener('click', $.fn.switchView);
});