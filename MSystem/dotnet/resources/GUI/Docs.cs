//���� ����� 
public static void AcceptPasport(Player player)
//�����             
string work = (acc.WorkID > 0) ? Jobs.WorkManager.JobStats[acc.WorkID] : "�����������";
//���������
string wedding = "� �����";
if (acc.MarriageName == null && acc.MarriageSurname == null) wedding = "--";
//����� ���� 
List<object> data = new List<object>
//����� 
gender,
//�����
wedding,