'use strict'

// 初始化信息
window.onload = function () {
    if ($('#user_power', parent.document).val() !== "超级管理员") {
        parent.layer.alert("权限不足！");
        parent.layui.admin.events.closeThisTabs();
    }
};

layui.config({
    base: '/Scripts/Project/UserInfo/' //静态资源所在路径
}).use(['form', 'table'], function () {
    var form = layui.form
        , table = layui.table;

    //用户列表数据表格
    table.render({
        elem: '#table_user',
        height: 500,
        width: 1000,
        url: '/UserInfo/GetUser', //数据接口
        page: true,//开启分页
        cols: [[
             { field: 'Id', title: '编号', fixed: 'left' }
            , { field: 'U_Name', title: '用户名'}
            , { field: 'Phone', title: '手机号',width: 150}
            , {
                field: 'Balance', title: '余额', width: 100, templet: function (d) {
                    if (d.balance < 0) {
                        return '<span style="color: red;">' + d.Balance + '</span>';
                    } else {
                        return d.Balance;
                    }

                }
            }
            , {
                field: 'Level', title: '级别', width: 100, templet: function (d) {
                    if (d.Level == 1) {
                        return "普通会员";
                    }
                    if (d.Level == 2) {
                        return "中级会员";
                    }
                    if (d.Level == 3) {
                        return "高级会员";
                    }
                    return "普通用户";
                }
            }
            , {
                field: 'Create_Time', title: '创建时间', width: 160, templet: function (d) {
                    return d.Create_Time.substring(0, 10);
                }
            }
            ,{ fixed: 'right', title: '操作', align: 'center', toolbar: '#toolbar', width: 300 }
        ]]
    });

    //监听"新增用户"按钮
    window.btn_addUser = function () {
        //编辑时再次弹框数据清除
        $("#UserForm")[0].reset();
        $('#userId').val(0);
        $('#u_name').attr("readonly", false);
        $('#balance').attr("readonly", false);
        layer.open({
            type: 1, //页面层
            title: "新增用户",
            area: ['500px', '300px'],
            btn: ['保存', '取消'],
            btnAlign: 'c', //按钮居中
            content: $('#div_addUser'),
            success: function (layero, index) {// 弹出layer后的回调函数,参数分别为当前层DOM对象以及当前层索引
                // 解决按回车键重复弹窗问题
                $(':focus').blur();
                // 为当前DOM对象添加form标志
                layero.addClass('layui-form');
                // 将保存按钮赋予submit属性
                layero.find('.layui-layer-btn0').attr({
                    'lay-filter': 'btn_saveUserAdd',
                    'lay-submit': ''
                });
                // 表单验证
                form.verify();
                // 刷新渲染(否则开关按钮不会显示)
                form.render();
            },
            yes: function (index, layero) {// 确认按钮回调函数,参数分别为当前层索引以及当前层DOM对象
                form.on('submit(btn_saveUserAdd)', function (data) {//data按name获取
                    console.log(data.field);
                    $.ajax({
                        type: 'post',
                        url: '/UserInfo/AddUser',
                        dataType: 'json',
                        data: data.field,
                        success: function (res) {
                            if (res.code === 200) {
                                layer.alert("新增成功!", function (index) {
                                    window.location.reload();
                                });
                            }
                            else if (res.code === 402) {
                                layer.alert("已存在该账号!");
                            }
                        }
                    });
                    return false;
                });
            }
        });
    }

    //监听表格工具栏
    table.on('tool(table_user)', function (obj) {
        var data = obj.data;
        if (obj.event === 'edit') {
            $('#id').val(data.Id);
            $('#u_name').val(data.U_Name);
            $('#phone').val(data.Phone);
            $('#balance').val(data.Balance);
            $('#balance').attr("readonly", true);
            layer.open({
                type: 1, //页面层
                title: "修改用户",
                area: ['500px', '300px'],
                btn: ['保存', '取消'],
                btnAlin: 'c', //按钮居中
                content: $('#div_addUser'),
                success: function (layero, index) {// 弹出layer后的回调函数,参数分别为当前层DOM对象以及当前层索引
                    // 解决按回车键重复弹窗问题
                    $('focus').blur();
                    // 为当前DOM对象添加form标志
                    layero.addClass('layui-form');
                    // 将保存按钮赋予submit属性
                    layero.find('.layui-layer-btn0').attr({
                        'lay-filter': 'btn_saveUserEdit',
                        'lay-submit': ''
                    });
                    // 表单验证
                    form.verify();
                    // 刷新渲染(否则开关按钮不会显示)
                    form.render();
                },
                yes: function (index, layero) {// 确认按钮回调函数,参数分别为当前层索引以及当前层DOM对象
                    form.on('submit(btn_saveUserEdit)', function (data) {//data按name获取
                        $.ajax({
                            type: 'post',
                            url: '/UserInfo/EditUser',
                            dataType: 'json',
                            data: data.field,
                            success: function (res) {
                                if (res.code === 200) {
                                    layer.alert("修改成功!", function (index) {
                                        window.location.reload();
                                    });
                                }
                            }
                        });
                    });
                }
            });
        }
        else if (obj.event === 'del') {
            layer.confirm('确认删除该账户?', function () {
                $.getJSON('/UserInfo/DelUser', { userId: data.Id }, function (res) {
                    if (res.code === 200) {
                        layer.alert("删除成功!", function success() {
                            window.location.reload();
                        });
                    }
                    else {
                        layer.alert("删除失败!");
                    }
                });
            });
        }
    });

    //监听查询按钮
    $("#search").click(function () {
        table.reload('table_user', {
            where: { search: $('#input').val() }
        });
    });

    //监听数据备份按钮
    window.btn_exportData = function () {
        layer.confirm('确认备份当前数据?', function (index) {
            layer.close(index);
            window.open('/UserInfo/ExportData');
        });
    }
});