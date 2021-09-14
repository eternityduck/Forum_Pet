import React, { Component } from 'react';


export class TopicPage extends Component{
    static displayName = TopicPage.name;
    
    constructor(props) {
        super(props);
        this.state = { topics: [], loading: true };
    }

    componentDidMount() {
        this.populateTopicData();
    }

    static renderTopicTable(topics) {
        return (
            <table className='table table-striped' aria-labelledby="tabelLabel">
                <tbody>
                {topics.topicList.map(topic =>
                <tr>
                    <td>
                        <div className="forumData">
                            <div className="forumTitle">
                                <a href={`/topic-act/${topic.id}`}>{topic.name}</a>
                            </div>
                            
                        </div>
                    </td>
                    <td>
                        <div className="forumpostcount">
                            {topic.numberOfPosts} Posts
                        </div>
                        <div className="forumMemberCount">
                            {topic.numberOfUsers} Users
                        </div>
                    </td>
                    <td>
                        <div className="forumDesc">
                            {topic.description}
                        </div>
                    </td>
                </tr>
                )}
                </tbody>
            </table>
        );
    }

    render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : TopicPage.renderTopicTable(this.state.topics);
        return (
            <div>
                <p>This component demonstrates fetching data from the server.</p>
                {contents}
            </div>
        );
    }
    async populateTopicData() {
        const response = await fetch('topic');
        const data = await response.json();
        this.setState({ topics: data, loading: false });
    }
}