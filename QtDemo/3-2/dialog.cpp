#include "dialog.h"

Dialog::Dialog(QWidget *parent)
    : QDialog(parent)
{
    //设定对话框大小
    resize(150,150);
    //新建QLabel标签
    label=new QLabel(this);
    //设置文字内容
    label->setText("Hello Qt");
    //设置标签的位置和大小
    label->setGeometry(0,0,100,100);
}

Dialog::~Dialog()
{

}
