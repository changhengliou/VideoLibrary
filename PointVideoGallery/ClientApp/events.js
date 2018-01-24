// import React from 'react';
// import ReactDom from 'react-dom';
import 'bootstrap-table';
import 'bootstrap-table/dist/bootstrap-table.css';
import 'select2';
import 'select2/dist/css/select2.css';
import './css/events.css';
import { setTableViewZhTwLocal, isEmpty, tableSetting } from './js/utils';

/**
 * map data to select2 format
 * @param {Array} arr 
 */
const mapSelectData = (arr) => arr.map(obj => {
        obj.id = obj.id || obj.Id;
        obj.text = obj.text || obj.Name;
        return obj;
    });

/**
 * Given 2 array with old data and new data, return the difference
 * @param {Array} oldArr 
 * @param {Array} newArr 
 * @returns {Object} @param {Array} add data sholud be add, @param {Array} rm data should be remove
 */
const getDiff = (oldArr, newArr) => {
    const add = [], rm = [];
    var oi = 0, ni = 0;
    oldArr = oldArr.sort((x,y) => x > y);
    newArr = newArr.sort((x,y) => x > y);
    while (oi < oldArr.length && ni < newArr.length) {
        if (oldArr[oi] == newArr[ni]) {
            oi++;
            ni++;
        } else if (oldArr[oi] > newArr[ni]) {
            add.push(newArr[ni]);
            ni++;
        } else {
            rm.push(oldArr[oi]);
            oi++;
        }
    }
    for (; oi < oldArr.length; ++oi)
        rm.push(oldArr[oi]);
    for (; ni < newArr.length; ++ni)
        add.push(newArr[ni]);
    return { add: add, rm: rm };
}

$(document).ready(() => {
    var table = $('#table'),
        locTable = $('#location-table'),
        soTable = $('#so-table'),
        resTable = $('#resource-table');

    // set i18n to zh-tw
    setTableViewZhTwLocal($);
    // event list table
    table.bootstrapTable({
        ...tableSetting,
        onClickCell: (field, value, row, element) => {
            if (field !== 'Edit')
                return;
        },
        url: '/api/v1/ad/events/',
        columns: [{
            field: 'Name',
            title: '名稱'
        }, {
            field: 'Edit',
            title: '編輯',
            formatter: (value, row, index, field) => `<button class='btn btn-sm btn-default' data-id="${row.Id}" onClick='$.fn.switchView(this);'>
                                                          <span class="glyphicon glyphicon-pencil"></span>
                                                      </button>`
        }]
    });   
    locTable.bootstrapTable({
        ...tableSetting,
        showRefresh: false,
        columns: [{
            field: 'Name',
            title: '名稱'
        }, {
            field: 'Remove',
            title: '刪除',
            formatter: (value, row, index, field) => `<button class='btn btn-sm btn-default' data-id="${row.Id}" data-loc onClick='$.fn.remove(this);'>
                                                          <span class="glyphicon glyphicon-remove"></span>
                                                      </button>`
        }]
    });
    soTable.bootstrapTable({
        ...tableSetting,
        showRefresh: false,
        columns: [{
            field: 'Name',
            title: '名稱'
        }, {
            field: 'Remove',
            title: '刪除',
            formatter: (value, row, index, field) => `<button class='btn btn-sm btn-default' data-id="${row.Id}" data-so onClick='$.fn.remove(this);'>
                                                          <span class="glyphicon glyphicon-remove"></span>
                                                      </button>`
        }]
    });

    // initialize query select box
    $('#locationSelect').select2({
        width: window.innerWidth > 700 ? '60%' : '90%',
        data: mapSelectData($._loc)
    });
    $('#soSelect').select2({
        width: window.innerWidth > 700 ? '60%' : '90%',
        data: mapSelectData($._so)
    });

    /**
     * Get detail data from server
     * @param {number} id request data id  
     */
    const requestData = (id) => {
        $.ajax({
            method: 'GET',
            url: `/api/v1/ad/events/${id}`
        })
        .done(res => {
            $._res = res;
            $('#location-table').bootstrapTable('load', res.LocationTags);
            $('#so-table').bootstrapTable('load', res.SoSettings);
        })
        .fail(err => {
            console.log(err);
        });
    }

    /**
     * Switch view between edit and view panel
     */
    $.fn.switchView = (e) => {
        var id;
        const edit = document.getElementById('editPanel'),
              view = document.getElementById('viewPanel');
        if (e instanceof HTMLElement) {
            id = e.getAttribute('data-id');
            requestData(id);
        }
        
        if (edit.style.display === '') {
            edit.setAttribute('data-id', null);
            edit.style.display = 'none';
            view.style.display = '';
        } else {
            edit.setAttribute('data-id', id);
            edit.style.display = '';
            view.style.display = 'none';
        }
        // a workaround to make scroll bar always visible
        window.dispatchEvent(new Event('resize'));
    }

    /**
     * query for ad events
     * @param {event} e 
     */
    $.fn.query = (e) => {
        const _data = {
            so: $('#soSelect').val(),
            location: $('#locationSelect').val()
        };
        if (_data.so.length === 0 && _data.location.length === 0) {
            $('#table').bootstrapTable('refresh');
            return;
        }
        $.ajax({
            url: '/api/v1/ad/events/q',
            method: 'POST',
            data: _data
        })
        .done(res => {
            $('#table').bootstrapTable('load', res);
        })
        .fail(err => {
            var msg = document.getElementById('msgBox');
            msg.style.display = 'block';
            msg.innerHTML = "查詢失敗!";
        });
    }

    /**
     * when add new row button is click
     */
    $.fn.addRow = (e) => {
        var _opt,
            _data,
            _select = $("#editSelect"),
            node = e.target.tagName === 'SPAN' ? e.target.parentElement : e.target,
            btn = document.getElementById('editSubmit');

        if (node.id === 'newLocBtn') {
            _opt = $._loc;
            _data = $._res.LocationTags.map(s => s.Id);
            btn.setAttribute('data-trigger', 'loc')
        } else if (node.id === 'newSoBtn') {
            _opt = $._so;
            _data = $._res.SoSettings.map(s => s.Id);
            btn.setAttribute('data-trigger', 'so')
        } else
            return;

        _select.empty();
        _select.select2({
            width: '100%',
            data: mapSelectData(_opt)
        });
        _select.val(_data).trigger('change');
        $("#editModal").modal('toggle');
    }

    /**
     * when update row button is click
     */
    $.fn.updateRow = (e) => {
        if (!(e.target instanceof HTMLElement))
            return;
        
        var target = e.target.getAttribute('data-trigger'),
            _data,
            _newData,
            _type;
        
            if (target === 'loc') {
            _data = $._res.LocationTags.map(o => o.Id);
            _type = 'location';
        } else if (target === 'so') {
            _data = $._res.SoSettings.map(o => o.Id);
            _type = 'so';
        } else 
            return;
        
        $.ajax({
            url: '/api/v1/ad/events/',
            method: 'PUT',
            data: {
                ...getDiff(_data, $('#editSelect').val()),
                type: _type,
                id: document.getElementById('editView').getAttribute('data-id')
            }
        })
        .done(res => {
            console.log(res);
            $("#editModal").modal('toggle');
        })
        .fail(err => {

        });
    }

    // add event listener to go back button
    document.getElementById('goBack').addEventListener('click', $.fn.switchView);
    // add listener to query button
    document.getElementById('queryBtn').addEventListener('click', $.fn.query);
    // add listener when add new row button is click
    document.getElementById('newLocBtn').addEventListener('click', $.fn.addRow);
    document.getElementById('newSoBtn').addEventListener('click', $.fn.addRow);
    // add listener when update new row button is click
    document.getElementById('editSubmit').addEventListener('click', $.fn.updateRow);
});