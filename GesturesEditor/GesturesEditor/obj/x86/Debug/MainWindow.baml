   M S B A M L   `   `   `  ����  GesturesEditor    GesturesEditor.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  GesturesEditor��5   	   $	��350��6    $	��525��6-   .��5      � .+�5   
   �� -   5        $��bLoadNetworks��5   c   $��Load networks��6   $��23��6)   $
��Left=�65   $��	52,33,0,0q�6P   $	��Top=�6x   $	��153��6�   6�   �� 5   
   -   5        $��listBoxGesturesBase��5   N   $	��100��6   $
��Left=�6   $��
68,105,0,0q�6:   $	��Top=�6i   $	��120��6�    �� DisplayMemberPath$
 Name��6�   $ �� IsSynchronizedWithCurrentItem$
 TrueY�6�   ��6�   ��  �� Path$ GestureMetadataContainers���5	      �� 5
       �� 
TargetType" ��6    �� BasedOn# [B��6:   .�5      -   5        5      5      �� 5   
   -   5        $��listBoxGesturesNames��5   O   $	��100��6   $
��Left=�6   $��333,105,0,0q�6:   $	��Top=�6k   $	��120��6�   $ GestureName��6�   $
 TrueY�6  ��6�   �� $# SelectedItem.GesturesMetadata� �� ElementName$ listBoxGesturesBase����5      �� 5      " ��6   # [B��6:   .�5      -   5        5      5      �� 5   
   -   5        $��bNewNetwork��5   h   $��Add a new network��6   $��23��6-   $
��Left=�69   $��
52,232,0,0q�6T   $	��Top=�6{   $	��153��6�   6�   �� 5   
   -   5        $��bRemoveNetwork��5   n   $��Remove selected network��6   $��23��63   $
��Left=�6?   $��
52,273,0,0q�6Z   $	��Top=�6�   $	��153��6�   6�   �� 5   
   -   5        $��	bSaveToDB��5   b   $��
Save to DB��6   $��64��6&   $
��Left=�62   $��319,232,0,0q�6M   $	��Top=�6s   $��153                 ��6�    � DataContext 5      �� 6+   �� 5   
   -	   5        $��label1��5   ^   $��	Networks:��6   $��28��6$   $
��Left=�60   $��	96,74,0,0q�6K   $	��Top=�6l   �� 5   
   -
   5        $��label2��5   K   $��Gestures names:��6   $��28��6*   $��346,74,61,0q�66   $	��Top=�6Y   5      5      