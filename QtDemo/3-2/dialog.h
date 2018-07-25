#ifndef DIALOG_H
#define DIALOG_H

#include <QDialog>
// 添加标签头文件
#include<QLabel>



class Dialog : public QDialog
{
    Q_OBJECT
    // 添加标签指针
    QLabel *label;

public:
    Dialog(QWidget *parent = 0);
    ~Dialog();
};

#endif // DIALOG_H
