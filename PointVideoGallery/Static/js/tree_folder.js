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
                        name: '舌尖上的中國 <div class="tree-actions"></div>', type: 'folder', additionalParameters: { id: 'F11' },
                        data: [
                            {
                                name: '第一季 <div class="tree-actions"></div>', type: 'folder', additionalParameters: { id: 'F12' },
                                data: [
                                         { name: '<i class="fa fa-folder"></i> 自然的饋贈', type: 'item', additionalParameters: { id: 'I12' } },
                                         { name: '<i class="fa fa-folder"></i> 主食的故事', type: 'item', additionalParameters: { id: 'I12' } },
                                         { name: '<i class="fa fa-folder"></i> 轉化的靈感', type: 'item', additionalParameters: { id: 'I12' } },
                                         { name: '<i class="fa fa-folder"></i> 時間的味道', type: 'item', additionalParameters: { id: 'I12' } },
                                         { name: '<i class="fa fa-folder"></i> 廚房的秘密', type: 'item', additionalParameters: { id: 'I12' } },
                                         { name: '<i class="fa fa-folder"></i> 五味的調和', type: 'item', additionalParameters: { id: 'I12' } },
                                         { name: '<i class="fa fa-folder"></i> 我們的田野', type: 'item', additionalParameters: { id: 'I12' } }
                                ]
                            },
                            { name: '第二季 <div class="tree-actions"></div>', type: 'folder', additionalParameters: { id: 'F13' } }
                        ]
                    },
                    {
                        name: '兩個爸爸 <div class="tree-actions"></div>', type: 'folder', additionalParameters: { id: 'F12' }
                    },
                    {
                        name: '微電影 <div class="tree-actions"></div>', type: 'folder', additionalParameters: { id: 'F12' },
                        data: [
                            { name: '<i class="fa fa-folder"></i> 12星座之第一天失戀劇場', type: 'item', additionalParameters: { id: 'I12' } },
                            { name: '<i class="fa fa-folder"></i> 110米欄的天空', type: 'item', additionalParameters: { id: 'I12' } }
                        ]
                    },
                    {
                        name: '美人心計 <div class="tree-actions"></div>', type: 'folder', additionalParameters: { id: 'F12' }
                    },
                    {
                        name: '後宮甄嬛傳 <div class="tree-actions"></div>', type: 'folder', additionalParameters: { id: 'F12' }
                    }
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