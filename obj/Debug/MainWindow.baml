   M S B A M L   `   `   `  ����K  GCSSpring2022MTM2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:CSSpring2022MTM2CSSpring2022MTM2  5   	   !    CSSpring2022MTM2.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:CSSpring2022MTM2     5   	   -   5          � Title$  CS499 MTM Study - Team 2��5   	   $	��650��5	   	   $	��750��6    � 	Resources 5         	UACrimson �� Color 
 UAGrey �� DataType(   "CSSpring2022MTM2.GestureResultView � RowDefinitions �� StringFormat��%?  &	        &	 #     (�� F     5      *, +)�� 5   
   $ 	#ffa51e36��6,   �� 5   
   $ 	#ff7e888e��6)   �� 5   
   "  6   .R�5      � $
��Auto��6   $��5q�6    ��6+   �� *	BodyColor+ 5      �� 5      $
��Auto �6$   �� 5      $
��Auto �6$   .+�5      �� $��16(�6R   $��Segoe UI)�6`   $��SemiBold"�6v   $��0��6�   ��6   �� *
Detected+$ Seated: {0}��6�   �� 5      $��16(�6X   $��Segoe UI)�6f   $��SemiBold"�6|   $��1��6�   ��6   �� *
Confidence+$ Confidence: {0}��6�   5      5      5      .��5      �  5   
   �� 5       $
��Auto �6   �� 5!      $��* �6   �� 5"      $
��Auto �6   .+�5&   
   �� $��0��6   $&�� Images/OfficLogo-Engineering.png��6   $��Right=�6G   $��48��6c   $��0 5 5 5q�6o   �� 5'   
   $��0��6   $
��Left=�6!   $��5 0 0 0q�6<   $��Center=�6M   #��[  6h   $��Segoe UI)�6�   $��18(�6�   MTM Study - CS499 Group 26�   � 5*   
   -   5         �Name$ contentGrid��5*      $��1��6$    � ColumnDefinitions 5+      �� 5,      $��3* �6#   �� 5-      $��1* �6#    5/      �� 50      �� 51      �� 52      �� 53      �� 54      �� 55      .+�58      �� $��10��6   $��0��6+   $��10q�6;   $��Center=�6G   $��Center=�6d   .+�59      (� -   5         (�Name$ kinectBodyViewbox��59      .��5:      �� $��UniformToFill=�6;   ��6   �� *ImageSource+6;   5;      �� 5<      $����Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed lacinia neque eget tellus lobortis, et tincidunt risus bibendum. Nulla porta tristique tellus, at consequat erat efficitur vitae. Nulla eu mauris.��6   $
��Wrap=�6�   $��5 10 5 5q�6  #��[ 6  $��14(�6>  $��Segoe UI)�6L  5=      5>      �� 5A   
   -   5        $��	statusBar��5A   ?   $��2��6   $��Stretch=�6!   $��Bottom=�6P   
�������.@�5B      �� $��
Finish me!��6   5C      5D      5E      