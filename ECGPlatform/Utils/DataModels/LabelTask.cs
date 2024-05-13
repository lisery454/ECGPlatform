namespace ECGPlatform;

public class LabelTask
{
    public LabelTask(int id, string title, List<int> fragmentIds, string description)
    {
        Id = id;
        Title = title;
        FragmentIds = fragmentIds;
        Description = description;
    }
    
    public int Id { get; set; }

    public string Title { get; set; }
    
    public List<int> FragmentIds { get; set; }
    
    public string Description { get; set; }
    
    
}