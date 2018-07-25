#include "dialog.h"

Dialog::Dialog(QWidget *parent)
    : QDialog(parent)
{
    //设置对话框的尺寸
    resize(300,300);
    //定义标签名字及其所在父窗口
    label=new QLabel("label",this);
    //定义按钮名字及其所在父窗口
    btn=new QPushButton("Click Me",this);
    //定义label和btn的位置
    label->move(150,150);
    btn->move(125,110);
    //关联信号和槽 实现关闭标签的功能
    connect(btn,SIGNAL(clicked()),label,SLOT(close()));

}

Dialog::~Dialog()
{

}
