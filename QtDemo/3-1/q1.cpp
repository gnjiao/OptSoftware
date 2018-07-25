//应用类对象头文件
#include<QApplication>
//对话框类头文件
#include<QDialog>
//标签类头文件
#include<QLabel>

 int main(int argc ,char *argv[])
 {
     //新建QApplication类对象,用于管理应用程序的各种设置
     //并执行事件处理工作,任何一个Qt GUI程序都要有一个QApplication对象
     //该对象需要argc和argv 2个参数
     QApplication a (argc,argv);
     //新建一个QDialog对象,实现一个对话框界面
     QDialog w;
     //新建一个QLabel对象,并将QDialog对象w作为参数,表面对象w是它的父窗口
     //也就是说这个标签放在对话框中
     QLabel label(&w);
     //给标签设置要显示的文字
     label.setText("Hello Qt");
     //设置标签相对于对话框的位置和大小
     label.setGeometry(10,10,100,100);
    //显示对话框
     w.show();
     //exec函数让QApplication对象进入时间循坏
     //这样QT应用程序在运行时便可以接受产生事件
     return a.exec();

 }
