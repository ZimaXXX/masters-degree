   M S B A M L   `   `   `  ����A  =WPFApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:WPFAppWPFApp  5           WPFApp.CircularMinuteTimer   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      'Runnerclr-namespace:WPFApp     5      -   5          >�Name$  circularMinuteTimer��5       � 	Resources 5      %   WPFApp.ProgressToAngleConverter   ProgressConverter  ActivateEllipse �� 
TargetName �� TargetProperty O� KeyTime O� Value  DeactivateEllipse �� 
TargetType �� Property �� Value	 �� 
TargetType   
WPFApp.Arc
   
StartAngle `� 	Converter �� Path �� RelativeSource   EndAngle��%�  &	        &	      &	 �     '� m    7   7     5	   
   6*   �� 5
   
   .��5      U� $ MainEllipse��5      $ (Ellipse.Opacity)�5      .\�5      n� $ 00:00:00.001��5      $	 0.8Q�5      5      5      �� 5   
   .��5      U� $ MainEllipse��5      $ (Ellipse.Opacity)�5      .\�5      n� $ 00:00:00.001��5      $	 0.5Q�5      5      5      �� 5   
   " �6   .�5      ��  � �� 5      �� 5      "	 �6&   .R�5       � .+�5"        $��15��6)   #��z��5#      $��-5q�5$      
 5%   "   `� 5&   &   8   .1�5'   *   �� $ Minimum�62    5(      �� *TemplatedParent+�� 5)   *   $ .�62    5*      �� *TemplatedParent+5+   '    5-   "   `� 5.   &   8  .1�5/   *   �� $ Value�62    50      �� *TemplatedParent+�� 51   *   $ .�62    52      �� *TemplatedParent+53   '   55      58      59      5;      5<      5>      .��5@      � .+�5A   
   A� -   5         A�Name$ MainEllipse��5A   �   ��6   �� $ Width� �� Source$ circularMinuteTimer����6K   �� $ Height�$ circularMinuteTimer��6�   ]� 5B   
   -   5         ]�Name$ MainDockPanel��5B      ��6+   �� $ Width�$ circularMinuteTimer����6d   �� $ Height�$ circularMinuteTimer��.+�5C      (� $��15q�5D      $��Stretch=�5E      $
��Auto��5F      .��5G      �� -   5         ��Name$ ViewBoxStackPanel��5G      .+�5H      �� 
�������$��Center=�5J      $��Center=�5K      $
��Bold"�5L      ��5H       �� $ Caption� �� ElementName$ circularMinuteTimer��5L      5M      5N      5P      � 5Q   
   -   5         �Name$ 	_progress��5Q       � Minimum$ 0Q�5R   
    � Maximum$ 60Q�5S   
    � Value$ 0Q�5T   
   ���#7f7f7f
�������5V   "   5X       � Triggers 5Z      :� 5[   
    :� RoutedEvent$" Runner:RutedEvent.startTimer��6   .[�5\      �� #��[ 6   �� 5]      -   5         ��Name$ StrokeTimer��5]      .��5^      �� -   5        .��5_      W�  W� From$ 0X�6&   	 W� To$ 61X�5`   
   $ 	_progress��5a   
   $ Value�5b   
    g� Duration$ 0:0:1D�5c   
   5d      5e      5f      :� 5g   
   $! Runner:RutedEvent.stopTimer��6   .[�5h      �� #��[ 6   �� 5i       �� BeginStoryboardName$ StrokeTimer��6   5j      5l      