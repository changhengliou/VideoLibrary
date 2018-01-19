import 'bootstrap-table';
import 'bootstrap-table/dist/bootstrap-table.css';
import './css/resource.css';
import { getDateTimeString, setTableViewZhTwLocal } from './js/utils';

// edit button click
const onEditClick = (row) => {
    var type = document.getElementById('editType'),
        name = document.getElementById('editName');
    document.getElementById('editUpdate').setAttribute('data-id', row.Id);
    document.getElementById('editDelete').setAttribute('data-id', row.Id);
    type.value = row.MediaType;
    name.value = row.Name;
}
(function ($) {
    setTableViewZhTwLocal($);
    $('#resource-table').bootstrapTable({
        onClickCell: (field, value, row, element) => {
            if (field !== 'Edit')
                return;
            onEditClick(row);
        },
        url: '/api/v1/ad/resource/table',
        sidePagination: 'server',
        iconSize: 'sm',
        locale: 'zh-TW',
        striped: true,
        pagination: true,
        pageNumber: 1,
        pageSize: 5,
        pageList: [5, 10, 25, 50],
        search: true,
        searchOnEnterKey: true, // search on enter press
        showHeader: true,
        showFooter: false,
        showRefresh: true,
        showToggle: false, //switch between cardView / detailView
        showPaginationSwitch: false, // show/hide pagination
        cardView: false, // if true, switch to card view
        detailView: false, // if true, show plus sign with detail enabled
        rowStyle: (row, index) => { return { css: { "vertical-align": "middle" } } },
        columns: [{
            field: 'ThumbnailPath',
            title: '預覽',
            formatter: (value, row, index, field) => 
                       `<div class='preview-img table-thumbnail' style='background: url("/assets?p=${value}") no-repeat;'/>` 
        }, {
            field: 'MediaType',
            title: '類型',
            sortable: true
        }, {
            field: 'Name',
            title: '名稱',
            sortable: true
        }, {
            field: 'CreateTime',
            title: '創建日期',
            sortable: true,
            formatter: (value) => getDateTimeString(new Date(value))
        }, {
            field: 'Edit',
            title: '編輯',
            formatter: (value, row, index, field) => `<button class='btn btn-sm btn-default' data-id="${row.Id}" data-toggle="modal" data-target="#editModal">
                                                          <span class="glyphicon glyphicon-pencil"></span>
                                                      </button>`
        }]
    });
})(jQuery);

(function () {
    // click/delete listener
    const onUpdateOrDeleteBtnClick = (e) => {
        var btn = e.target.id,
            type = document.getElementById('editType').value,
            name = document.getElementById('editName').value,
            id = e.target.getAttribute('data-id');

        console.log(e.target);
        if (btn === 'editUpdate') {
            $.ajax({
                url: '/api/v1/ad/resource/update',
                method: 'PUT',
                data: {
                    Id: id,
                    Name: name,
                    MediaType: type
                }
            })
            .done(res => {
                $('#editModal').modal('toggle');
                $('#resource-table').bootstrapTable('refresh');
                console.log(res);
            })
            .fail(err => {
                console.log(err);
            });
        } else if (btn === 'editDelete') {
            $.ajax({
                url: `/api/v1/ad/resource/remove/${id}`,
                method: 'DELETE'
            })
            .done(res => {
                $('#editModal').modal('toggle');
                $('#resource-table').bootstrapTable('refresh');
                console.log(res);
            })
            .fail(err => {
                console.log(err);
            });
        }
    }
    // register click/delete click listener
    document.getElementById('editUpdate').addEventListener('click', onUpdateOrDeleteBtnClick);
    document.getElementById('editDelete').addEventListener('click', onUpdateOrDeleteBtnClick);

    // upload button click listener
    document.getElementById('upload-img').addEventListener('change', (e) => {
        var input = e.target;
        if (input.files && input.files.length) {
            toggleBtn();
            for(var i = 0; i < input.files.length; ++i) {
                if (!input.files[i].type.match('image')) continue;
                // read file as preview
                var reader = new FileReader();
                reader.id = i;
                // add onload listener
                reader.onload = function(e) {
                    var imgBox = document.createElement('div');
                    imgBox.id = `pre_${e.target.id}`;
                    imgBox.className = 'panel col-lg-2 col-sm-3';
                    imgBox.innerHTML = `<div class='panel-body preview-img-container'>
                                            <span class="preview-img-abort" id="abort_${e.target.id}" data-id="${e.target.id}">×</span>
                                            <div style='background: url("${e.target.result}") no-repeat;' class='preview-img'></div>
                                            <select id="type" class="form-control input-sm" form='resourceUpload' name='type_${e.target.id}' data-id=${e.target.id}>
                                                <option value="image">Image</option>
                                                <option value="video">Video</option>
                                                <option value="webpage">WebPage</option>
                                            </select>
                                            <input class='form-control input-sm' placeholder='name' type='text' style='margin-top: 8px;' name='name_${e.target.id}' data-id=${e.target.id} data-fname=${input.files[e.target.id].name}></input>
                                        </div>`;
                    document.getElementById('preview-panel').append(imgBox);
                    document.getElementById(`abort_${e.target.id}`).addEventListener('click', onAbortPreviewBtnClick);
                }

                reader.readAsDataURL(input.files[i]);
            }
        }
    });
    // submit button click listener
    document.getElementById('submit').addEventListener('click', (e) => {
        var data = new FormData(),
            fileInfo = {},
            upload = document.getElementById('upload-img');

        $('#preview-form [data-id]').each((index, e) => {
            if (e.tagName.toLowerCase() === 'input')
                fileInfo[e.getAttribute('data-id')] = { ...fileInfo[e.getAttribute('data-id')], name: e.value, filename: e.getAttribute('data-fname') };
            else if (e.tagName.toLowerCase() === 'select')
                fileInfo[e.getAttribute('data-id')] = { ...fileInfo[e.getAttribute('data-id')], type: e.value };
        });

        data.append('fileInfo', JSON.stringify(fileInfo));

        Object.keys(fileInfo).map(key => {
            data.append("files[]", upload.files[key]);
        });

        $.ajax({
            url: '/api/v1/ad/resource/upload',
            method: 'POST',
            processData: false,
            contentType: false,
            data: data
        })
        .done(msg => {
            console.log(msg);
            toggleBtn();
            clearPreviewPanel();
            $("#resource-table").bootstrapTable('refresh');
        })
        .fail(err => console.log(err));
    });

    // show / hide submit and upload button 
    const toggleBtn = () => {
        var upload = document.querySelector('label[for="upload-img"]'),
            submit = document.getElementById('submit');
        if (upload.style.display === '') {
            upload.style.display = 'none';
            submit.style.display = '';
        } else {
            upload.style.display = '';
            submit.style.display = 'none';
        }
    }

    // clear all content on preview panel
    const clearPreviewPanel = () => {
        document.getElementById('preview-panel').innerHTML = '';
        document.getElementById('upload-img').value = '';
    }

    // when user don't want to upload this 
    const onAbortPreviewBtnClick = (e) => {
        var id = e.target.getAttribute('data-id');
        var ele = document.getElementById(`pre_${id}`);
        document.getElementById(`abort_${id}`).removeEventListener('click', onAbortPreviewBtnClick);
        ele.parentNode.removeChild(ele);
    }
})();