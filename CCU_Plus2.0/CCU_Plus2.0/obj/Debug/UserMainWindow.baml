   M S B A M L   `   `   `  ����F  BCCU_Plus2.0, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:CCU_Plus2._0CCU_Plus2._0  5   	   !    CCU_Plus2._0.UserMainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:CCU_Plus2._0     5   	   S OMaterialDesignThemes.Wpf, Version=1.0.1.0, Culture=neutral, PublicKeyToken=nullFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5        � Title$  
ChatOnline��5	   	    � Icon$ Icon/chat.ico��6   $	��600��61   $
��1080��6>   
������� � 
ResizeMode$ NoResize=�6^    � WindowStartupLocation$ CenterScreen=�6t    � WindowStyle$
 None=�6�    � 	Resources 5
       �� 
TargetType   Button1 �� Property �� Value	 �� 
TargetType
 �� CornerRadius �� EndPoint �� 
StartPoint � Color � Offset  Textbox1 ��Name ��Name �� Triggers P� Property P� Value��%  &	        &	 �    �� 5   
   " ��6   .�5      ��  � ��$ 13(�6)   �� 5       � ��
 �����6+   �� 5       � ��
 �����6+   �� 5       � �� 5      �� 5      "	 ��6&   .R�5      �� $��PART_Background��6{   #��z��6!   $��0q�6U   $
 12��6i   ��5      �� 5   "   $	 0,1&�66   $	 0,0&�6E   .��5   &   � $
 Gray��63   $	 0.0Q�6@   5   #   ._�5      �� #��z��6/   $��Center=�6b   $��Center=�6   5      5      5      5      �� 5   
   " ��6   .�5       ��  � ��$ 7q�6(   �� 5!       � �� �#0006+   �� 5"       � �� 5#      �� 5$      "	 ��6&   .R�5%      �� $ border��6!   $
 15��5&   !   
�������$��2q�5(   !   
�������$��	10,0,10,0q�5)   8   ._�5*      �� $ PART_ContentHost��6+   |�.  $��Hidden=�6W   $��Hidden=�6~   5+       5,      P� 5-       �@J�IsMouseOver . 5.      P� 5/       �@J�	IsFocused . 50      52      54      55      56      .��57      � ���	#FFDEDEDE.+�58      � $��60��6	   $	��Top=�6   ���	#FF122D48 J� Effect 59      3  -System.Windows.Media.Effects.DropShadowEffect  5:         
BlurRadius$ 1Q�6      ShadowDepth$ 1Q�6&      Color$ 	#FFACACAC��66   .+�5<      ��  �� Orientation$ 
Horizontal=�6   $��Right=�6)   ���	#FF122D48.+�5=      �� -   5        $��UserName��5=      $
��User��6    $��Center=�6,   $��16(�6G   '  !MaterialDesignThemes.Wpf.PopupBox  5>         PlacementMode$ BottomAndAlignRightEdges=�6      	StaysOpen .  $��10q�6Y   ���	#FF122D48   PopupContent. 5?      �� $	��150��6   ���	#FF122D48.+�5@      �� -   5        $��GoChat��5@      $��ChatRoom��6   
�������6E   �� 5A      -   5        $��	GoAccount��5A      $��Account��6    
�������6J   �� 5B      -   5        $��	GoContact��5B      $��Contact��6    
�������6J   �� 5C      �� 5D      -   5        $��GoOut��5D      $��Logout��6   
�������6A   5E      5F      5G      �� 5H      -   5        $ 
Horizontal=�5H      $��Center=�6)   .+�5I      �� $��Center=�6   $��Center=�6-   $��22(�6H   �� 6W   $�Ano Talk��6[   6n   5J      �� 5K      -   5         ��Name$ image��5K      $
��Left=�6   $��60��65   $	��Top=�6A   $��71��6Y   5L      � 5M      -   5         �Name$ GridMain��5M   	   $��Right=�6   $��Bottom=�67   $	��540��6R   $
��1080��6_   .+�5N       � -	   5        $��frame��5N      $�� ��6     � NavigationUIVisibility$ Hidden=�6#   $
��Left=�6C   $	��457��6^   $��
110,10,0,0q�6k   $	��Top=�6   $	��845��6�   5O      � 5P      $��60��6	   $��Bottom=�6   ���	#FF122D48 5Q        5R      $ 1Q�6   $ 1Q�6&   $ 	#FFACACAC��66   .+�5T      �� $ 
Horizontal=�6   $��Right=�6)   �� 5U      -
   5        $��SendBox��5U      $
��Left=�6   $��Center=�67   $��40��6Y   $��	10,10,0,0q�6e    �� TextWrapping$
 Wrap=�6x   $�� ��6�   $	��Top=�6�   $	��887��6�   $��20(�6�   #��[ 6�   �� 5V      -   5        $��SendBTN��5V      $
��SEND��6   $
��Left=�6*   $��
902,10,0,0q�6E   $	��Top=�6Y   $	��168��6q   $��40��6}   $��18(�6�   #��[  6�   5W      5X      5Y      