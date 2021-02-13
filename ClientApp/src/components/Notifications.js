import React, { Component } from 'react';

export class Notifications extends Component {
  static displayName = Notifications.name;

  constructor(props) {
    super(props);
    this.state = { notifications: [], loading: true };
  }

  componentDidMount() {
    this.fetchData();
  }
  
  renderNotification(notification) {
    return (
      <tr key={notification.id}>
        <td>{notification.time}</td>
        <td>{notification.reason}</td>
      </tr>
    )
  }

  renderNotificationsTable(notifications) {
      return (
      <table className='table table-striped'>
        <thead>
          <tr>
            <th>Time</th>
            <th>Reason</th>
          </tr>
        </thead>
        <tbody>
          { notifications.map(this.renderNotification) }
        </tbody>
      </table>
    );
  }

  render() {
    let notifications = this.state.loading
      ? <p><em>Loading...</em></p>
      : this.renderNotificationsTable(this.state.notifications);

    return (
      <div>
        <h1>Delivered Notifications</h1>
        {notifications}
      </div>
    );
  }

  async fetchData() {
    const response = await fetch('api/notifications');
    const data = await response.json();
    this.setState({ notifications: data, loading: false });
  }
}
