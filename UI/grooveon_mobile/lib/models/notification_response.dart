import 'package:json_annotation/json_annotation.dart';

part 'notification_response.g.dart';

@JsonSerializable()
class NotificationResponse {
  final int id;
  final int userId;
  final String title;
  final String content;
  final String type;
  final bool isRead;
  final DateTime createdAt;

  NotificationResponse({
    required this.id,
    required this.userId,
    required this.title,
    required this.content,
    required this.type,
    required this.isRead,
    required this.createdAt,
  });

  factory NotificationResponse.fromJson(Map<String, dynamic> json) =>
      _$NotificationResponseFromJson(json);

  Map<String, dynamic> toJson() => _$NotificationResponseToJson(this);
}
