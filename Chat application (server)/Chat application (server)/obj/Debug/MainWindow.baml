   M S B A M L   `   `   `  ����T  PChat application (server), Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:Chat_application__server_Chat_application__server_  5   	   *    $Chat_application__server_.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   9local'clr-namespace:Chat_application__server_     5   	     � Title$  Chat application server��5   	   $��170.334��5	   	   $��320.066��5
   	   .��5      � ��5   
   �� 5       �� EndPoint$ 0.5,1&�6"    �� 
StartPoint$ 0.5,0&�5   "   .��5      �  � Color$ 	#FF512DA8��6    � Offset$ 0Q�5      � 5      $ 	#FF311B92��6   $ 1Q�5      5      .+�5   
   �� .+�5      �� $��وضعیت سرور��6   $��fonts/#Vazir)�5      
������� �� TextAlignment$ Center=�5      $��30��5      $��20(�5      $	��Top=�5      $��0,10,0,0q�5      �� 5      -   5         ��Name$ statusTextBlock��5       $��غیر فعال��5      $��fonts/#Vazir)�5      
�������$ Center=�5"      $��30��5#      $��15(�5$      $��0,10,0,0q�5%      $	��Top=�5&      �� 5'      -   5         ��Name$ startServerButton��5-      $ ��روشن کردن سرور��5'      $��fonts/#Vazir)�5(      $��40��5)      $	��130��5*      
���   �$��12(�5,      $��SemiBold"�5.      ��50      �� 51       �� 
TargetType" ��6   .�52      �� 	 �� Property	 � ��
 �� Value$
 fonts/#IRANSANS)�53   !   �� 54      	 � ��

 �����55   !   �� 56      	 �@�Cursor$

 Hand��57   !   �� 58      	 �@J�Opacity$	
 0.7Q�59   !    �� Triggers 5;      P� 5<       P� Property �@J�IsMouseOver P� Value . . �5>   "   �� 	 � ��
 �#e6e6e65?   )   5@      5C      ��5E      �� 5F       �� 
TargetType" ��6&   .R�5G      �� $	��130��6!   $��35��5H   !    �� CornerRadius$ 15��5I   !   #��z��5J   !   ._�5K      �� $��Center=�6/   $��Center=�5L   /   5M      5N      5P      5Q      5R      5T      