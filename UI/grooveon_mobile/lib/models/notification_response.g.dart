// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'notification_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

NotificationResponse _$NotificationResponseFromJson(
  Map<String, dynamic> json,
) => NotificationResponse(
  id: (json['id'] as num).toInt(),
  userId: (json['userId'] as num).toInt(),
  title: json['title'] as String? ?? 'Notification',
  content: json['content'] as String? ?? '',
  type: json['type'] as String? ?? 'general',
  isRead: json['isRead'] as bool? ?? false,
  createdAt: DateTime.tryParse(json['createdAt'] as String? ?? '') ??
      DateTime.now(),
);

Map<String, dynamic> _$NotificationResponseToJson(
  NotificationResponse instance,
) => <String, dynamic>{
  'id': instance.id,
  'userId': instance.userId,
  'title': instance.title,
  'content': instance.content,
  'type': instance.type,
  'isRead': instance.isRead,
  'createdAt': instance.createdAt.toIso8601String(),
};
