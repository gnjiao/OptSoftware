#ifndef DIALOG_H
#define DIALOG_H

#include <QDialog>
//添加标签头文件
#include <QLabel>
//添加按钮标签头文件
#include<QPushButton>

class Dialog : public QDialog
{
    Q_OBJECT
private:
    //声明一个标签指针
    QLabel *label;
    //声明一个按钮指针
    QPushButton *btn;




public:
    Dialog(QWidget *parent = 0);
    ~Dialog();
};

#endif // DIALOG_H
