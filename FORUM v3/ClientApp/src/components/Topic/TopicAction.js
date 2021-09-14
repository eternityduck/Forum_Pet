import React, { Component } from 'react';
import {
    useParams
} from "react-router-dom";



export class TopicAction extends Component{
    static displayName = TopicAction.name;

    constructor(props) {
        super(props);
        this.state = { topic: [], loading: true };
        
    }

    componentDidMount() {
        this.populateTopicData(this.id);
    }

    static renderTopicTable(topic) {
        return (
            <table className="table table-hover table-bordered" id="forumIndexTable">
                <tbody>
                {topic.posts.map(post =>
                    <tr>
                        <td>
                            <div className="postData">
                                <div className="postTitle">
                                    <a href={`/post-act/${post.id}`}>{post.title}</a>
                                </div>
                                <div className="forumSubTitle">
                                    <div>
                                            <span className="postAuthor">
                                                <a href={`/profile/${post.authorId}`}>{post.author}</a>
                                            </span>
                                    </div>
                                </div>
                            </div>
                        </td>
                        <td>
                            <div className="forumPostCount">
                                Replies: {post.repliesCount}
                            </div>
                        </td>
                        <td>
                            <div className="postDate">{post.datePosted}</div>
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
            : TopicAction.renderTopicTable(this.state.topics);
        return (
            <div>
                <p>This component demonstrates fetching data from the server.</p>
                {contents}
            </div>
        );
    }
    async populateTopicData(id) {
        const response = await fetch(`topic/${id}`);
        const data = await response.json();
        this.setState({ topic: data, loading: false });
    }
}