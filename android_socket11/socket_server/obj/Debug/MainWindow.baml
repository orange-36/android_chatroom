   M S B A M L   `   `   `  ����H  Dsocket_server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:socket_serversocket_server  5   	       socket_server.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:socket_server     5   	     � Title$  Socket Server��5   	   $	��600��6   $	��800��6,   .��5	      (�  (� Stretch$
 Fill=�6   .��5
   
   �� $	��800��6   $	��600��6   .+�5      �� -   5         ��Name$ Socket_state��5      $
��Left=�6,   $��50��6G   $��	20,20,0,0q�6S    �� TextWrapping$
 Wrap=�6f   $��Socket State : ��6z   $	��Top=�6�   $	��210��6�   $��30(�6�   $
��Bold"�6�   $��Center=�6�   $��Center=�6�   $��0q�6  �� 5      -   5        $ socket_state_tbox��5      $
��Left=�61   $��50��6L   $��
250,20,0,0q�6X   $
 Wrap=�6l   $��Disconnected��6�   $	��Top=�6�   $	��200��6�   $��30(�6�   $��Center=�6�   $��Center=�6�   $��0q�6  �� 5      -   5        $ 
system_msg��5      $
��Left=�5      $	��450��5      $��	20,90,0,0q�5      $
 Wrap=�5      $�� ��5      $	��Top=�5      $	��750��5      $��18(�5      $��0q�5      ���	#FFE8E8E86&   �� 5      -   5        $��start_server��5      $	��140��6*   $��50��66   $��
470,20,0,0q�6B   $��Start server��6V   $
��Left=�6n   $	��Top=�6�   $��24(�6�   6�   �� 5      -   5        $��
end_server��5      $	��140��6(   $��50��64   $��
630,20,0,0q�6@   $��	Shut down��6T   $
��Left=�6i   $	��Top=�6�   $��24(�6�   6�   5      5      5      