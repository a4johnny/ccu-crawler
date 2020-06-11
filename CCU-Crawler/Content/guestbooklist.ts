class guestbook {
    public Content: string;
    public CourceId: string;
    public DateTime: Date;
    public Id: string;
    public Score: string;
}
class guestbooklistModel {
    public Guestbooklist: Array<guestbook> = [];
}

class guestbooklistView {
    protected Model: guestbooklistModel;

    constructor(m: guestbooklistModel) {
        this.Model = m;
        this.CreateView();
        this.Update();
    }
    private CreateView() {

    }
    public Update() {
        //   console.log(e[0].Score);
        this.Model.Guestbooklist.forEach((value, index, Array) => {
            console.log('index[' + index + ']: ' + value.Score + ', ' + value.CourceId);
        });

    }
}


class guestbooklistController {
    protected Model: guestbooklistModel;
    protected View: guestbooklistView;

    constructor() {
        this.Model = new guestbooklistModel;
        this.View = new guestbooklistView(this.Model);
        this.SubscribeEvents();
        this.Main();
    }
    private SubscribeEvents() {

    }

    private Main() {
                $.get('/api/Guestbooks/', (e) => this.GetGuestbooks(e));
            }
            private GetGuestbooks(e: Array<guestbook>) {
                this.Model.Guestbooklist = e;
                this.View.Update();
    }
}

$().ready(() => {
    let app = new guestbooklistController();
});
