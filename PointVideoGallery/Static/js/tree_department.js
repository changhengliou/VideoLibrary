var TreeView = function () {

    return {
        //main function to initiate the module
        init: function () {

            var DataSourceTree = function (options) {
                this._data = options.data;
                this._delay = options.delay;
            };

            DataSourceTree.prototype = {

                data: function (options, callback) {
                    var self = this;

                    setTimeout(function () {
                        if (options.data == undefined) {
                            if (self.isroot == undefined) {
                                var data = $.extend(true, [], self._data);
                                callback({ data: data });
                                self.isroot = true;
                            } else {
                                callback({ data: [] });
                            }
                        } else {
                            data = $.extend(true, [], options.data);
                            callback({ data: data });
                        }

                    }, this._delay)
                }
            };

            // INITIALIZING TREE
            var treeDataSource = new DataSourceTree({
                data: [
                    {
                        name: '董事會 <div class="tree-actions"></div>', type: 'folder', additionalParameters: { id: 'F11' },
                        data: [
                            {
                                name: '總經理室 <div class="tree-actions"></div>', type: 'folder', additionalParameters: { id: 'F12' },
                                data: [
                                         { name: '<i class="fa fa-user"></i> Charmmy', type: 'item', additionalParameters: { id: 'I11' } },
                                         { name: '<i class="fa fa-user"></i> Doris', type: 'item', additionalParameters: { id: 'I12' } },
                                         { name: '<i class="fa fa-user"></i> Emma', type: 'item', additionalParameters: { id: 'I12' } }
                                ]
                            },
                            { name: '<i class="fa fa-user"></i> Betty', type: 'item', additionalParameters: { id: 'I11' } },
                            { name: '<i class="fa fa-user"></i> Kitty', type: 'item', additionalParameters: { id: 'I12' } }
                        ]
                    },
                    {
                        name: '系統資訊總處 <div class="tree-actions"></div>', type: 'folder', additionalParameters: { id: 'F12' },
                        data: [
                                    { name: '<i class="fa fa-user"></i> Andy', type: 'item', additionalParameters: { id: 'I12' } },
                                    { name: '<i class="fa fa-user"></i> Howard', type: 'item', additionalParameters: { id: 'I12' } },
                                    { name: '<i class="fa fa-user"></i> Kevin', type: 'item', additionalParameters: { id: 'I12' } },
                                    { name: '<i class="fa fa-user"></i> Lucas', type: 'item', additionalParameters: { id: 'I12' } },
                                    { name: '<i class="fa fa-user"></i> Peter', type: 'item', additionalParameters: { id: 'I12' } }
                        ]
                    },
                    {
                        name: '技術研發處 <div class="tree-actions"></div>', type: 'folder', additionalParameters: { id: 'F12' },
                        data: [
                            { name: '<i class="fa fa-user"></i> Mathrew', type: 'item', additionalParameters: { id: 'I12' } },
                            { name: '<i class="fa fa-user"></i> Irene', type: 'item', additionalParameters: { id: 'I12' } },
                            { name: '<i class="fa fa-user"></i> Clair', type: 'item', additionalParameters: { id: 'I12' } },
                            { name: '<i class="fa fa-user"></i> Ivy', type: 'item', additionalParameters: { id: 'I12' } },
                            { name: '<i class="fa fa-user"></i> Lily', type: 'item', additionalParameters: { id: 'I12' } }
                        ]
                    },
                    {
                        name: '客戶服務處 <div class="tree-actions"></div>', type: 'folder', additionalParameters: { id: 'F12' },
                        data: [
                            { name: '<i class="fa fa-user"></i> Mathrew', type: 'item', additionalParameters: { id: 'I12' } },
                            { name: '<i class="fa fa-user"></i> Irene', type: 'item', additionalParameters: { id: 'I12' } },
                            { name: '<i class="fa fa-user"></i> Clair', type: 'item', additionalParameters: { id: 'I12' } },
                            { name: '<i class="fa fa-user"></i> Ivy', type: 'item', additionalParameters: { id: 'I12' } },
                            { name: '<i class="fa fa-user"></i> Lily', type: 'item', additionalParameters: { id: 'I12' } }
                        ]
                    },
                    { name: '<i class="fa fa-user"></i> Jack', type: 'item', additionalParameters: { id: 'I12' } }
                ],
                delay: 400
            });

            
            $('#FlatTree').tree({
                dataSource: treeDataSource,
                loadingHTML: '<img src="img/input-spinner.gif"/>',
            });

        }

    };

}();